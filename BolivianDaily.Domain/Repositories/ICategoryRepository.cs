namespace BolivianDaily.Domain.Repositories;

using BolivianDaily.Domain.Entities;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
