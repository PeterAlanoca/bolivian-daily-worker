using BolivianDaily.CheckerWorker.Domain.Entities;

namespace BolivianDaily.CheckerWorker.Domain.Repositories;

public interface IProcessedArticleRepository
{
    Task<bool> ExistsForSourceArticleAsync(long sourceArticleId, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedArticle article, CancellationToken cancellationToken = default);
}
