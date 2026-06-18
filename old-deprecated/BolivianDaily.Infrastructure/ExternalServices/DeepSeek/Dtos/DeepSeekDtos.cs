using System.Text.Json.Serialization;

namespace BolivianDaily.Infrastructure.ExternalServices.DeepSeek.Dtos;

public class DeepSeekChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "deepseek-chat";

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.2;

    [JsonPropertyName("messages")]
    public List<DeepSeekChatMessageDto> Messages { get; set; } = new();
}

public class DeepSeekChatMessageDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class DeepSeekChatResponse
{
    [JsonPropertyName("choices")]
    public List<DeepSeekChatChoiceDto> Choices { get; set; } = new();
}

public class DeepSeekChatChoiceDto
{
    [JsonPropertyName("message")]
    public DeepSeekChatMessageDto Message { get; set; } = new();
}
