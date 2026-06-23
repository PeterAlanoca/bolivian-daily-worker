using System.Text;
using System.Text.Json;
using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.CheckerWorker.Infrastructure.Messaging;

public sealed class RabbitMqArticleProcessedEventPublisher(IOptions<RabbitMqOptions> options) : IArticleProcessedEventPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task PublishAsync(ArticleProcessedEvent message, CancellationToken cancellationToken = default)
    {
        var rabbitMqOptions = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqOptions.HostName,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.UserName,
            Password = rabbitMqOptions.Password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(rabbitMqOptions.ArticleProcessedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(
            rabbitMqOptions.ArticleProcessedQueue,
            rabbitMqOptions.Exchange,
            RabbitMqTopology.ArticleProcessedRoutingKey(rabbitMqOptions));

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonOptions));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = nameof(ArticleProcessedEvent);

        channel.BasicPublish(
            rabbitMqOptions.Exchange,
            RabbitMqTopology.ArticleProcessedRoutingKey(rabbitMqOptions),
            mandatory: false,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}
