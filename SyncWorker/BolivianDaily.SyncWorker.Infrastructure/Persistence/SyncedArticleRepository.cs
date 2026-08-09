using BolivianDaily.SyncWorker.Domain.Entities;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.SyncWorker.Infrastructure.Persistence;

public sealed class SyncedArticleRepository(SyncDbContext context) : ISyncedArticleRepository
{
    public Task<bool> IsSyncedAsync(long checkedArticleId, CancellationToken cancellationToken = default)
    {
        return context.SyncedArticles.AnyAsync(
            article => article.CheckedArticleId == checkedArticleId && article.Status == "SYNCED",
            cancellationToken);
    }

    public async Task AddAsync(SyncedArticle syncedArticle, CancellationToken cancellationToken = default)
    {
        await context.SyncedArticles.AddAsync(syncedArticle, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
