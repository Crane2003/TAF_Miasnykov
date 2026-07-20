using System.Text.Json.Serialization;

namespace Business.Models;

public class DashboardUpdateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("updateWidgets")]
    public List<Widget> UpdateWidgets { get; set; } = new();
}
