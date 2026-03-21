namespace BolivianDaily.Domain.Repositories;

using BolivianDaily.Domain.Entities;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
