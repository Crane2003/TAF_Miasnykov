using Newtonsoft.Json;

namespace TAF.Business.Models;

public class Widget
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("configuration")]
    public Dictionary<string, object> Configuration { get; set; } = new();

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
            Type = "chart",
            Position = 0,
            Width = 6,
            Height = 4,
            Configuration = new Dictionary<string, object>
            {
                { "chartType", "line" },
                { "dataSource", "default" }
            }
        };
    }

    public static Widget CreateChartWidget(string name, int position)
    {
        return new Widget
        {
            Name = name,
            Type = "chart",
            Position = position,
            Width = 6,
            Height = 4
        };
    }

    public static Widget CreateTableWidget(string name, int position)
    {
        return new Widget
        {
            Name = name,
            Type = "table",
            Position = position,
            Width = 12,
            Height = 6
        };
    }
}
