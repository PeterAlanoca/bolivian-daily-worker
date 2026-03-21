namespace BolivianDaily.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Domain.Repositories;
using BolivianDaily.Infrastructure.Data;

public class NewsRepository : INewsRepository
{
    private readonly ApplicationDbContext _context;

    public NewsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<News?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.News
            .Include(n => n.Multimedia)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<News?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _context.News
            .Include(n => n.Multimedia)
            .FirstOrDefaultAsync(n => n.Url == url, cancellationToken);
    }

    public async Task AddAsync(News news, CancellationToken cancellationToken = default)
    {
        await _context.News.AddAsync(news, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(News news, CancellationToken cancellationToken = default)
    {
        _context.News.Update(news);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _context.News.AnyAsync(n => n.Url == url, cancellationToken);
    }
}
