using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public class News : AuditableEntity
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("source_id")]
    public long? SourceId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("pretitle")]
    public string? Pretitle { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("enter")]
    public string? Enter { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("publication_date")]
    public DateTime? PublicationDate { get; set; }

    [JsonPropertyName("multimedia")]
    public List<Multimedia> Multimedia { get; set; } = new();

    // Navigation properties for Entity Framework Core
    [JsonIgnore]
    public Source? Source { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }
}
