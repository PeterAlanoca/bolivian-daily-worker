using BolivianDaily.SyncWorker.Domain.Entities;

namespace BolivianDaily.SyncWorker.Domain.Repositories;

public interface ISyncedArticleRepository
{
    Task<bool> IsSyncedAsync(long checkedArticleId, CancellationToken cancellationToken = default);
    Task AddAsync(SyncedArticle syncedArticle, CancellationToken cancellationToken = default);
}
