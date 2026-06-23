using BolivianDaily.SyncWorker.Domain.Entities;

namespace BolivianDaily.SyncWorker.Domain.Repositories;

public interface IArticleSyncLogRepository
{
    Task<bool> IsSyncedAsync(Guid processedArticleId, CancellationToken cancellationToken = default);
    Task AddAsync(ArticleSyncLog syncLog, CancellationToken cancellationToken = default);
}
