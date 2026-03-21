using System.Text.Json.Serialization;

namespace BolivianDaily.Domain.Entities;

public class Category : AuditableEntity
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonIgnore]
    public List<SourceCategory> Sources { get; set; } = new();

    [JsonIgnore]
    public List<News> News { get; set; } = new();
}
