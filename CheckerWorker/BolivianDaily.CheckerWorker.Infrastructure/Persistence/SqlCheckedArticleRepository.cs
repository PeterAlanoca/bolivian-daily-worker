using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.CheckerWorker.Infrastructure.Persistence;

public sealed class SqlCheckedArticleRepository(CheckerDbContext context) : ICheckedArticleRepository
{
    public Task<bool> ExistsForScrapedArticleAsync(long scrapedArticleId, CancellationToken cancellationToken = default)
    {
        return context.CheckedArticles.AnyAsync(
            article => article.ScrapedArticleId == scrapedArticleId,
            cancellationToken);
    }

    public async Task AddAsync(CheckedArticle article, CancellationToken cancellationToken = default)
    {
        await context.CheckedArticles.AddAsync(article, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
