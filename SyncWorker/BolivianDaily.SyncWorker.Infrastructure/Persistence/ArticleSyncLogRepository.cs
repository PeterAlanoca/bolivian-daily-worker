using BolivianDaily.SyncWorker.Domain.Entities;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.SyncWorker.Infrastructure.Persistence;

public sealed class ArticleSyncLogRepository(SyncDbContext context) : IArticleSyncLogRepository
{
    public Task<bool> IsSyncedAsync(long checkedArticleId, CancellationToken cancellationToken = default)
    {
        return context.ArticleSyncLogs.AnyAsync(
            log => log.CheckedArticleId == checkedArticleId && log.Status == "Synced",
            cancellationToken);
    }

    public async Task AddAsync(ArticleSyncLog syncLog, CancellationToken cancellationToken = default)
    {
        await context.ArticleSyncLogs.AddAsync(syncLog, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
