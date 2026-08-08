using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.CheckerWorker.Infrastructure.Messaging;

public sealed class RabbitMqArticleCheckedEventPublisher(
    IOptions<RabbitMqOptions> options,
    IConnection connection,
    ILogger<RabbitMqArticleCheckedEventPublisher> logger)
    : RabbitMqEventPublisher<ArticleCheckedEvent>(
        options,
        connection,
        logger,
        options.Value.ArticleCheckedQueue,
        options.Value.ArticleCheckedRoutingKey),
      IArticleCheckedEventPublisher
{
}
