namespace BolivianDaily.CheckerWorker.Infrastructure.Configuration;

public sealed class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}
