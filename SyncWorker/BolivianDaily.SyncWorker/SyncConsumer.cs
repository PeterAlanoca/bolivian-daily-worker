using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;
using Microsoft.Extensions.Options;

namespace BolivianDaily.SyncWorker;

public sealed class SyncConsumer(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<SyncConsumer> logger) : RabbitMqConsumerHostedService<ArticleCheckedEvent>(serviceProvider, options, logger)
{
    protected override string QueueName => Options.Value.ArticleCheckedQueue;

    protected override string RoutingKey => Options.Value.ArticleCheckedRoutingKey;

    protected override async Task HandleAsync(ArticleCheckedEvent message, CancellationToken stoppingToken)
    {
        Logger.LogInformation("Syncing checked article {ScrapedArticleId} -> {CheckedArticleId} ({Title})", message.ScrapedArticleId, message.CheckedArticleId, message.Title);

        using var scope = ServiceProvider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<SyncCheckedArticleUseCase>();
        await useCase.ExecuteAsync(message, stoppingToken);

        Logger.LogInformation("Article {CheckedArticleId} synced and acked", message.CheckedArticleId);
    }
}
