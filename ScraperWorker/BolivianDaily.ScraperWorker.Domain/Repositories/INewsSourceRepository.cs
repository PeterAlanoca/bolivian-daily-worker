using BolivianDaily.ScraperWorker.Domain.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Repositories;

public interface INewsSourceRepository
{
    Task<NewsSource?> GetActiveByAliasAsync(string alias, CancellationToken cancellationToken = default);
}
