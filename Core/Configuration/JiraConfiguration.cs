namespace Core.Configuration;

public class JiraConfiguration
{
    public bool Enabled { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public string PassedTransitionName { get; set; } = "Passed";
    public string FailedTransitionName { get; set; } = "Failed";
}
