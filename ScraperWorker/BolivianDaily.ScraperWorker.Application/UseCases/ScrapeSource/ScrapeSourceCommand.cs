namespace BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

public sealed record ScrapeSourceCommand(
    string SourceAlias,
    int CategoryDelayMs,
    int MinArticleDelayMs,
    int MaxArticleDelayMs
);
