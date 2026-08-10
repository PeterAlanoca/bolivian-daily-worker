using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class SqlArticleRepository(ScraperDbContext scraperDbContext) : IArticleRepository
{
    public async Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await scraperDbContext.Articles.AnyAsync(a => a.Url == url, cancellationToken);
    }

    public async Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        await scraperDbContext.Articles.AddAsync(article, cancellationToken);
        await scraperDbContext.SaveChangesAsync(cancellationToken);
    }
}
