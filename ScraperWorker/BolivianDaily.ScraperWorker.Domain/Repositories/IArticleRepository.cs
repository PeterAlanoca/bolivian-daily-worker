using BolivianDaily.ScraperWorker.Domain.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Repositories;

public interface IArticleRepository
{
    Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task AddAsync(Article article, CancellationToken cancellationToken = default);
}
