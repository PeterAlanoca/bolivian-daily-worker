using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public class SourceCategory : AuditableEntity
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("source_id")]
    public int SourceId { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonIgnore]
    public Source? Source { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }
}
