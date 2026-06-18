using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class InMemoryArticleRepository : IArticleRepository
{
    private readonly List<Article> _articles = new();
    private long _nextId = 1;

    public Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var exists = _articles.Any(article => string.Equals(article.Url, url, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        article.Id = _nextId++;
        _articles.Add(article);
        return Task.CompletedTask;
    }
}
