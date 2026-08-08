using BolivianDaily.CheckerWorker.Application.UseCases.CheckScrapedArticle;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;

namespace BolivianDaily.CheckerWorker;

public sealed class CheckerConsumer(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<CheckerConsumer> logger) : RabbitMqConsumerHostedService<ArticleScrapedEvent>(serviceProvider, options, logger)
{
    protected override string QueueName => Options.Value.ArticleScrapedQueue;

    protected override string RoutingKey => Options.Value.ArticleScrapedRoutingKey;

    protected override async Task HandleAsync(ArticleScrapedEvent message, CancellationToken stoppingToken)
    {
        Logger.LogInformation("Checking scraped article {ArticleId} ({Title}) from {SourceName}", message.ArticleId, message.Title, message.SourceName);

        using var scope = ServiceProvider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<CheckScrapedArticleUseCase>();
        await useCase.ExecuteAsync(message, stoppingToken);

        Logger.LogInformation("Article {ArticleId} checked and acked", message.ArticleId);
    }
}
