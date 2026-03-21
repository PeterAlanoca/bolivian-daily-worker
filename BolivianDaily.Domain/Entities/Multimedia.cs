using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public class Multimedia : AuditableEntity
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("news_id")]
    public long? NewsId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    // Navigation property
    [JsonIgnore]
    public News? News { get; set; }
}
