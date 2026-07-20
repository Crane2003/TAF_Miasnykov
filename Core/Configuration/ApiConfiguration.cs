namespace Core.Configuration;

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = "http://localhost:8080/api/v1";
    public string AuthToken { get; set; } = string.Empty;
}
