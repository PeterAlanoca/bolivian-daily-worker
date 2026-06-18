namespace BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

public sealed record ScrapeSourceResult(
    string SourceAlias,
    int CategoriesProcessed,
    int ArticleUrlsFound,
    int ArticlesScraped,
    int ArticlesSkipped);
