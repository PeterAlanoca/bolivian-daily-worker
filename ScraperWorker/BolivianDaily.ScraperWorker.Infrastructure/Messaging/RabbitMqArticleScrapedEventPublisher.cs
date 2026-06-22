using System.Text;
using System.Text.Json;
using BolivianDaily.Shared.Messaging;
using BolivianDaily.ScraperWorker.Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.ScraperWorker.Infrastructure.Messaging;

public sealed class RabbitMqArticleScrapedEventPublisher(
    IOptions<RabbitMqOptions> options,
    IConnection connection) : IArticleScrapedEventPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task PublishAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default)
    {
        var rabbitMqOptions = options.Value;
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(rabbitMqOptions.ArticleScrapedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(
            rabbitMqOptions.ArticleScrapedQueue,
            rabbitMqOptions.Exchange,
            RabbitMqTopology.ArticleScrapedRoutingKey(rabbitMqOptions));

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonOptions));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = nameof(ArticleScrapedEvent);

        channel.BasicPublish(
            rabbitMqOptions.Exchange,
            RabbitMqTopology.ArticleScrapedRoutingKey(rabbitMqOptions),
            mandatory: false,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}
