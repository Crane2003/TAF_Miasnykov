namespace TAF.Core.Configuration;

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = "http://localhost:8080/api/v1";
    public int Timeout { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}
