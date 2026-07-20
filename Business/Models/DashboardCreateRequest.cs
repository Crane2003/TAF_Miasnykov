using System.Text.Json.Serialization;

namespace Business.Models;

public class DashboardCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    public static DashboardCreateRequest CreateDefault() => new()
    {
        Name = $"Test Dashboard {DateTime.Now:yyyyMMddHHmmss}",
        Description = "Automated test dashboard"
    };

    public static DashboardCreateRequest CreateWithName(string name) => new()
    {
        Name = name,
        Description = "Automated test dashboard"
    };
}
