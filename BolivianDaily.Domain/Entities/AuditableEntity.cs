using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public abstract class AuditableEntity
{
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
