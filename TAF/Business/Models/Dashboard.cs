using Newtonsoft.Json;

namespace TAF.Business.Models;

public class Dashboard
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("widgets")]
    public List<Widget> Widgets { get; set; } = new();

    [JsonProperty("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [JsonProperty("owner")]
    public string Owner { get; set; } = string.Empty;

    public Dashboard() { }

    public Dashboard(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void AddWidget(Widget widget)
    {
        Widgets.Add(widget);
    }

    public void RemoveWidget(string widgetId)
    {
        Widgets.RemoveAll(w => w.Id == widgetId);
    }

    public Widget? GetWidgetById(string widgetId)
    {
        return Widgets.FirstOrDefault(w => w.Id == widgetId);
    }

    public int GetWidgetCount()
    {
        return Widgets.Count;
    }

    public static Dashboard CreateDefault()
    {
        return new Dashboard
        {
            Name = $"Test Dashboard {DateTime.Now:yyyyMMddHHmmss}",
            Description = "Auto-generated test dashboard",
            Owner = "testuser"
        };
    }

    public static Dashboard CreateWithWidgets(int widgetCount)
    {
        var dashboard = CreateDefault();
        for (int i = 0; i < widgetCount; i++)
        {
            dashboard.AddWidget(Widget.CreateDefault($"Widget {i + 1}"));
        }
        return dashboard;
    }
}
