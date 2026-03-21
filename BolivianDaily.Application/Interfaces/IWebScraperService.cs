namespace BolivianDaily.Application.Interfaces;

using BolivianDaily.Domain.Entities;

public interface IWebScraperService
{
    Task<List<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default);
    Task<News?> ScrapeArticleAsync(string articleUrl, CancellationToken cancellationToken = default);
}
