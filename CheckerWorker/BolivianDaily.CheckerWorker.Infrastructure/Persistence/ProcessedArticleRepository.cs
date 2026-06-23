using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.CheckerWorker.Infrastructure.Persistence;

public sealed class ProcessedArticleRepository(CheckerDbContext context) : IProcessedArticleRepository
{
    public Task<bool> ExistsForSourceArticleAsync(long sourceArticleId, CancellationToken cancellationToken = default)
    {
        return context.ProcessedArticles.AnyAsync(
            article => article.SourceArticleId == sourceArticleId,
            cancellationToken);
    }

    public async Task AddAsync(ProcessedArticle article, CancellationToken cancellationToken = default)
    {
        await context.ProcessedArticles.AddAsync(article, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
