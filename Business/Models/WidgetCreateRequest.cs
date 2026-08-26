using System.Text.Json;
using System.Text.Json.Serialization;

namespace Business.Models;

/// <summary>
/// Payload for POST /{project}/widget, which creates a project-level widget. 
/// This is distinct from PUT /dashboard/{id}/add, which only
/// links an already-existing widget to a dashboard.
/// </summary>
public class WidgetCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("widgetType")]
    public string WidgetType { get; set; } = string.Empty;

    [JsonPropertyName("share")]
    public bool Share { get; set; }

    [JsonPropertyName("filterIds")]
    public List<int> FilterIds { get; set; } = [];

    [JsonPropertyName("contentParameters")]
    public WidgetContentParameters ContentParameters { get; set; } = new();
}

public class WidgetContentParameters
{
    private const int DefaultItemsCount = 50;

    [JsonPropertyName("itemsCount")]
    public int ItemsCount { get; set; } = DefaultItemsCount;

    [JsonPropertyName("contentFields")]
    public List<string> ContentFields { get; set; } = [];

    [JsonPropertyName("widgetOptions")]
    public Dictionary<string, JsonElement> WidgetOptions { get; set; } = new();

    /// <summary>
    /// Builds the creation-time content parameters a widget type requires.
    /// </summary>
    public static WidgetContentParameters ForWidgetType(string widgetType, IDictionary<string, JsonElement>? widgetOptions = null) => new()
    {
        ItemsCount = DefaultItemsCount,
        ContentFields = [.. GetContentFields(widgetType)],
        WidgetOptions = widgetOptions is null ? new() : new Dictionary<string, JsonElement>(widgetOptions)
    };

    /// <summary>
    /// Returns the content fields each widget type requires at creation time.
    /// </summary>
    private static string[] GetContentFields(string widgetType)
        => widgetType.ToLowerInvariant() switch
        {
            "launchestable" =>
                ["name", "status", "statistics$executions$total", "statistics$executions$passed", "statistics$executions$failed"],
            _ =>
                ["statistics$executions$total", "statistics$executions$passed", "statistics$executions$failed"]
        };
}

public class EntityCreatedResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
