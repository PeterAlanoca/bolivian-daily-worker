using BolivianDaily.ScraperWorker.Domain.Entities;

namespace BolivianDaily.ScraperWorker.Application.Interfaces;

public interface INewsSourceParser
{
    string SourceAlias { get; }
    Task<IReadOnlyCollection<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default);
    Task<Article?> ParseArticleAsync(string articleUrl, CancellationToken cancellationToken = default);
}
