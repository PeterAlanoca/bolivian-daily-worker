using System.Text.Json.Serialization;

namespace BolivianDaily.Application.Dtos;

public class NewsAnalysisResult
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("has_html")]
    public bool HasHtml { get; set; }

    [JsonPropertyName("is_incomplete")]
    public bool IsIncomplete { get; set; }

    [JsonPropertyName("category_correct")]
    public bool CategoryCorrect { get; set; }

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("is_enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("issues")]
    public List<string> Issues { get; set; } = new();
}
