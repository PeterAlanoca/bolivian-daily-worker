using System.Text.Json.Serialization;

namespace BolivianDaily.Infrastructure.ExternalApi.Dtos;

public class NewsSubmissionDto
{
    [JsonPropertyName("category_id")]
    public long? CategoryId { get; set; }

    [JsonPropertyName("source_id")]
    public long? SourceId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; } = 1;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("pretitle")]
    public string? Pretitle { get; set; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("enter")]
    public string? Enter { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("publication_date")]
    public string? PublicationDate { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = "A";

    [JsonPropertyName("multimedia")]
    public List<MultimediaSubmissionDto> Multimedia { get; set; } = new();
}

public class MultimediaSubmissionDto
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
