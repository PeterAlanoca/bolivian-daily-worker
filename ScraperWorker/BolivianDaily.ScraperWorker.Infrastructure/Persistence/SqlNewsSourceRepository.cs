using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class SqlNewsSourceRepository : INewsSourceRepository
{
    private readonly ScraperDbContext _context;

    public SqlNewsSourceRepository(ScraperDbContext context)
    {
        _context = context;
    }

    public async Task<NewsSource?> GetActiveByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await _context.NewsSources
            .Include(s => s.Categories)
            .FirstOrDefaultAsync(s => s.Alias == alias && s.State == "A", cancellationToken);
    }
}
