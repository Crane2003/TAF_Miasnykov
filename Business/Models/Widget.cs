using System.Text.Json;
using System.Text.Json.Serialization;

namespace Business.Models;

public class Widget
{
    [JsonPropertyName("widgetId")]
    public int? Id { get; set; }

    [JsonPropertyName("widgetName")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("widgetType")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("widgetSize")]
    public WidgetSize Size { get; set; } = new();

    [JsonPropertyName("widgetPosition")]
    public WidgetPosition Position { get; set; } = new();

    [JsonPropertyName("widgetOptions")]
    public Dictionary<string, JsonElement> Options { get; set; } = new();

    public Widget() { }

    public Widget(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public static Widget CreateDefault(string name)
    {
        return new Widget
        {
            Name = name,
            Type = "overallStatistics",
            Size = new WidgetSize { Width = 6, Height = 7 },
            Position = new WidgetPosition { PositionX = 0, PositionY = 0 },
            Options = new Dictionary<string, JsonElement>
            {
                { "viewMode", JsonSerializer.SerializeToElement("panel") },
                { "latest", JsonSerializer.SerializeToElement(false) }
            }
        };
    }

    public static Widget CreateChartWidget(string name, int positionX, int positionY)
    {
        return new Widget
        {
            Name = name,
            Type = "chart",
            Size = new WidgetSize { Width = 6, Height = 4 },
            Position = new WidgetPosition { PositionX = positionX, PositionY = positionY }
        };
    }

    public static Widget CreateTableWidget(string name, int positionX, int positionY)
    {
        return new Widget
        {
            Name = name,
            Type = "table",
            Size = new WidgetSize { Width = 12, Height = 6 },
            Position = new WidgetPosition { PositionX = positionX, PositionY = positionY }
        };
    }
}

public class WidgetSize
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

public class WidgetPosition
{
    [JsonPropertyName("positionX")]
    public int PositionX { get; set; }

    [JsonPropertyName("positionY")]
    public int PositionY { get; set; }
}
