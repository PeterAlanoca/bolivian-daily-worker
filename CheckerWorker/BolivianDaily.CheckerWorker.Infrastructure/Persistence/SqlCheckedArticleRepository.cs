using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.CheckerWorker.Infrastructure.Persistence;

public sealed class SqlCheckedArticleRepository(CheckerDbContext checkerDbContext) : ICheckedArticleRepository
{
    public Task<bool> ExistsForScrapedArticleAsync(long scrapedArticleId, CancellationToken cancellationToken = default)
    {
        return checkerDbContext.CheckedArticles.AnyAsync(
            checkedArticle => checkedArticle.ScrapedArticleId == scrapedArticleId,
            cancellationToken);
    }

    public async Task AddAsync(CheckedArticle checkedArticle, CancellationToken cancellationToken = default)
    {
        await checkerDbContext.CheckedArticles.AddAsync(checkedArticle, cancellationToken);
        await checkerDbContext.SaveChangesAsync(cancellationToken);
    }
}
