using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Interfaces;

public interface IAiArticleChecker
{
    Task<ProcessedArticle> CheckAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default);
}
