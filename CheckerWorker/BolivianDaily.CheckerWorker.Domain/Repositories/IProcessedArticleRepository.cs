using BolivianDaily.CheckerWorker.Domain.Entities;

namespace BolivianDaily.CheckerWorker.Domain.Repositories;

public interface IProcessedArticleRepository
{
    Task<bool> ExistsForScrapedArticleAsync(long scrapedArticleId, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedArticle article, CancellationToken cancellationToken = default);
}
