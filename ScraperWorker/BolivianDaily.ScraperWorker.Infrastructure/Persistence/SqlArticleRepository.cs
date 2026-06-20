using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class SqlArticleRepository : IArticleRepository
{
    private readonly ScraperDbContext _context;

    public SqlArticleRepository(ScraperDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _context.Articles.AnyAsync(a => a.Url == url, cancellationToken);
    }

    public async Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        await _context.Articles.AddAsync(article, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
