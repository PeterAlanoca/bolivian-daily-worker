namespace BolivianDaily.SyncWorker.Infrastructure.Configuration;

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";

    public string Url { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
