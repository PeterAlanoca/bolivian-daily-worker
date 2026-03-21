namespace BolivianDaily.Domain.Repositories;

using BolivianDaily.Domain.Entities;

public interface ISourceRepository
{
    Task<Source?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Source>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
