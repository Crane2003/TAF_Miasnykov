using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Configuration;

namespace Core.Api;

public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private readonly ILogger _logger;

    public ApiClient()
    {
        _logger = Log.ForContext<ApiClient>();

        var apiConfig = ConfigurationLoader.Instance.GetApiConfiguration();
        _baseUrl = apiConfig.BaseUrl.TrimEnd('/');

        var handler = new HttpClientHandler();

        if (IsLoopback(_baseUrl))
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        _client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        if (!string.IsNullOrEmpty(apiConfig.AuthToken))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiConfig.AuthToken);
        }

        _logger.Information("ApiClient initialized with base URL: {BaseUrl}", _baseUrl);
    }

    private static bool IsLoopback(string baseUrl)
        => Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) && uri.IsLoopback;

    public async Task<ApiResponse> ExecuteGetAsync(string endpoint, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, HttpMethod.Get, null, headers);

    public async Task<ApiResponse> ExecutePostAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, HttpMethod.Post, body, headers);

    public async Task<ApiResponse> ExecutePutAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, HttpMethod.Put, body, headers);

    public async Task<ApiResponse> ExecutePatchAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, HttpMethod.Patch, body, headers);

    public async Task<ApiResponse> ExecuteDeleteAsync(string endpoint, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, HttpMethod.Delete, null, headers);

    private async Task<ApiResponse> ExecuteAsync(string endpoint, HttpMethod method, object? body, IDictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing {Method} request to: {Endpoint}", method, endpoint);

        using var request = new HttpRequestMessage(method, BuildUri(endpoint));

        if (headers != null)
            foreach (var header in headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");

        using var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        _logger.Information("{Method} {Endpoint} - Status: {StatusCode}", method, endpoint, response.StatusCode);

        return new ApiResponse
        {
            StatusCode = response.StatusCode,
            Content = string.IsNullOrEmpty(content) ? null : content,
            IsSuccessful = response.IsSuccessStatusCode
        };
    }

    private string BuildUri(string endpoint)
        => $"{_baseUrl}/{endpoint.TrimStart('/')}";
}
