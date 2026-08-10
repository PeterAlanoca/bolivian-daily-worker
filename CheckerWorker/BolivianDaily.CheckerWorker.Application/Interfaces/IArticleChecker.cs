using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Interfaces;

public interface IArticleChecker
{
    Task<CheckedArticle> CheckAsync(ArticleScrapedEvent articleScrapedEvent, CancellationToken cancellationToken = default);
}
