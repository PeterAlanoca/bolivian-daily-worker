using BolivianDaily.CheckerWorker.Domain.Entities;

namespace BolivianDaily.CheckerWorker.Domain.Repositories;

public interface ICheckedArticleRepository
{
    Task<bool> ExistsForScrapedArticleAsync(long scrapedArticleId, CancellationToken cancellationToken = default);

    Task AddAsync(CheckedArticle checkedArticle, CancellationToken cancellationToken = default);
}
