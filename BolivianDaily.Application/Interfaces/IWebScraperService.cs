namespace BolivianDaily.Application.Interfaces;

using BolivianDaily.Domain.Entities;

public interface IWebScraperService
{
    Task<List<string>> GetLatestArticleUrlsAsync(Source source, Category category, CancellationToken cancellationToken = default);
    Task<News?> ScrapeArticleAsync(Source source, string articleUrl, CancellationToken cancellationToken = default);
}
