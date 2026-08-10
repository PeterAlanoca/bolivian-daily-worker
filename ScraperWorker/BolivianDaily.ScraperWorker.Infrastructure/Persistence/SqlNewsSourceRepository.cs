using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class SqlNewsSourceRepository(ScraperDbContext scraperDbContext) : INewsSourceRepository
{
    public async Task<NewsSource?> GetActiveByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await scraperDbContext.NewsSources
            .Include(s => s.Categories.Where(c => c.State == "A"))
            .ThenInclude(c => c.Category)
            .FirstOrDefaultAsync(
                s => s.Alias == alias && s.State == "A",
                cancellationToken);
    }
}
