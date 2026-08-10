using BolivianDaily.SyncWorker.Domain.Entities;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.SyncWorker.Infrastructure.Persistence;

public sealed class SyncedArticleRepository(SyncDbContext syncDbContext) : ISyncedArticleRepository
{
    public Task<bool> IsSyncedAsync(long checkedArticleId, CancellationToken cancellationToken = default)
    {
        return syncDbContext.SyncedArticles.AnyAsync(
            syncedArticle => syncedArticle.CheckedArticleId == checkedArticleId && syncedArticle.Status == "SYNCED",
            cancellationToken);
    }

    public async Task AddAsync(SyncedArticle syncedArticle, CancellationToken cancellationToken = default)
    {
        await syncDbContext.SyncedArticles.AddAsync(syncedArticle, cancellationToken);
        await syncDbContext.SaveChangesAsync(cancellationToken);
    }
}
