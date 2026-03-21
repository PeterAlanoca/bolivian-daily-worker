namespace BolivianDaily.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Domain.Repositories;
using BolivianDaily.Infrastructure.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
