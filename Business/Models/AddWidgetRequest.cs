using System.Text.Json.Serialization;

namespace Business.Models;

public class AddWidgetRequest
{
    [JsonPropertyName("addWidget")]
    public Widget AddWidget { get; set; } = null!;
}
