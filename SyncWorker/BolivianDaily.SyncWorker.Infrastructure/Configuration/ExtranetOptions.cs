namespace BolivianDaily.SyncWorker.Infrastructure.Configuration;

public sealed class ExtranetOptions
{
    public const string SectionName = "Extranet";

    public string Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; } = 180;
}
