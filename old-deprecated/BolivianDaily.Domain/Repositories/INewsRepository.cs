namespace BolivianDaily.Domain.Repositories;

using BolivianDaily.Domain.Entities;

public interface INewsRepository
{
    Task<News?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<News?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task AddAsync(News news, CancellationToken cancellationToken = default);
    Task UpdateAsync(News news, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default);
}
