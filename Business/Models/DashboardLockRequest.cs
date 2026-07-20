using System.Text.Json.Serialization;

namespace Business.Models;

public class DashboardLockRequest
{
    [JsonPropertyName("locked")]
    public bool Locked { get; set; }
}
