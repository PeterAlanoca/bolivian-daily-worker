using BolivianDaily.SyncWorker.Domain.Entities;

namespace BolivianDaily.SyncWorker.Domain.Repositories;

public interface IArticleSyncLogRepository
{
    Task<bool> IsSyncedAsync(long processedArticleId, CancellationToken cancellationToken = default);
    Task AddAsync(ArticleSyncLog syncLog, CancellationToken cancellationToken = default);
}
