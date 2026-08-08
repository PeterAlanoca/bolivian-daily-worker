using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.ScraperWorker.Infrastructure.Messaging;

public sealed class RabbitMqArticleScrapedEventPublisher(
    IOptions<RabbitMqOptions> options,
    IConnection connection,
    ILogger<RabbitMqArticleScrapedEventPublisher> logger)
    : RabbitMqEventPublisher<ArticleScrapedEvent>(
        options,
        connection,
        logger,
        options.Value.ArticleScrapedQueue,
        options.Value.ArticleScrapedRoutingKey),
      IArticleScrapedEventPublisher
{
}
