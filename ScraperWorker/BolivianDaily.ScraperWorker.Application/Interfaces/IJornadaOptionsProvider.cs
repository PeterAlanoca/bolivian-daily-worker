namespace BolivianDaily.ScraperWorker.Application.Interfaces;

public interface IJornadaOptionsProvider
{
    string Alias { get; }
    int IntervalMinutes { get; }
    int CategoryDelayMs { get; }
    int MinArticleDelayMs { get; }
    int MaxArticleDelayMs { get; }
}