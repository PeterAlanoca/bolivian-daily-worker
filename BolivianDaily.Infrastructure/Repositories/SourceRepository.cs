namespace BolivianDaily.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Domain.Repositories;
using BolivianDaily.Infrastructure.Data;

public class SourceRepository : ISourceRepository
{
    private readonly ApplicationDbContext _context;

    public SourceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Source?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sources
            .Include(s => s.Categories)
            .ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Source>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        // By default returning all, assuming all retrieved are active or we can add IsActive flag.
        return await _context.Sources
            .Include(s => s.Categories)
            .ThenInclude(sc => sc.Category)
            .ToListAsync(cancellationToken);
    }
}
