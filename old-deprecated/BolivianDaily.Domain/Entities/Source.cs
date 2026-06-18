using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public class Source : AuditableEntity
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("categories")]
    public List<SourceCategory> Categories { get; set; } = new();

    // Mapping for 1-to-N with News
    [JsonIgnore]
    public List<News> News { get; set; } = new();
}
