using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Configuration;

namespace Core.Jira;

public enum JiraTestOutcome
{
    Passed,
    Failed
}

public class JiraClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _client;
    private readonly JiraConfiguration _config;
    private readonly ILogger _logger;

    public JiraClient(JiraConfiguration config)
    {
        _config = config;
        _logger = Log.ForContext<JiraClient>();

        _client = new HttpClient
        {
            BaseAddress = new Uri(_config.BaseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        var authBytes = Encoding.ASCII.GetBytes($"{_config.Email}:{_config.ApiToken}");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task UpdateTestCaseStatusAsync(string issueKey, JiraTestOutcome outcome)
    {
        if (!_config.Enabled)
            return;

        var transitionName = outcome == JiraTestOutcome.Passed
            ? _config.PassedTransitionName
            : _config.FailedTransitionName;

        try
        {
            var transitionId = await ResolveTransitionIdAsync(issueKey, transitionName);
            if (transitionId == null)
            {
                _logger.Warning(
                    "Jira transition {TransitionName} not found for issue {IssueKey}",
                    transitionName, issueKey);
                return;
            }

            var payload = new { transition = new { id = transitionId } };
            using var content = new StringContent(
                JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");

            using var response = await _client.PostAsync($"rest/api/3/issue/{issueKey}/transitions", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.Information(
                    "Jira issue {IssueKey} transitioned to {TransitionName}", issueKey, transitionName);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.Warning(
                    "Failed to transition Jira issue {IssueKey} to {TransitionName}: {StatusCode} {Body}",
                    issueKey, transitionName, response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Error updating Jira issue {IssueKey} status", issueKey);
        }
    }

    private async Task<string?> ResolveTransitionIdAsync(string issueKey, string transitionName)
    {
        using var response = await _client.GetAsync($"rest/api/3/issue/{issueKey}/transitions");
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.Warning(
                "Failed to fetch Jira transitions for issue {IssueKey}: {StatusCode} {Body}",
                issueKey, response.StatusCode, body);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("transitions", out var transitions))
            return null;

        foreach (var transition in transitions.EnumerateArray())
        {
            if (transition.TryGetProperty("name", out var nameElement) &&
                string.Equals(nameElement.GetString(), transitionName, StringComparison.OrdinalIgnoreCase) &&
                transition.TryGetProperty("id", out var idElement))
            {
                return idElement.GetString();
            }
        }

        return null;
    }
}
