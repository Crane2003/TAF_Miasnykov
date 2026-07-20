using System.Text.Json.Serialization;

namespace Business.Models;

public class Dashboard
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("widgets")]
    public List<Widget> Widgets { get; set; } = new();

    public void AddWidget(Widget widget) => Widgets.Add(widget);

    public void RemoveWidget(int widgetId) => Widgets.RemoveAll(w => w.Id == widgetId);

    public Widget? GetWidgetById(int widgetId) => Widgets.FirstOrDefault(w => w.Id == widgetId);

    public int GetWidgetCount() => Widgets.Count;
}
