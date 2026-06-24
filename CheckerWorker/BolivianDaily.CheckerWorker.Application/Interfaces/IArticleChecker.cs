using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Interfaces;

public interface IArticleChecker
{
    Task<ProcessedArticle> CheckAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default);
}
