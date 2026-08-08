using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.Shared.Messaging;

public class RabbitMqEventPublisher<TMessage>(
    IOptions<RabbitMqOptions> options,
    IConnection connection,
    ILogger logger,
    string queueName,
    string routingKey) where TMessage : class
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        var rabbitMqOptions = options.Value;
        using var channel = connection.CreateModel();

        RabbitMqTopology.Declare(channel, rabbitMqOptions, queueName, routingKey);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonOptions));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = typeof(TMessage).Name;

        channel.BasicPublish(
            rabbitMqOptions.Exchange,
            routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        logger.LogInformation("Published {MessageType} event to queue {Queue}", typeof(TMessage).Name, queueName);

        return Task.CompletedTask;
    }
}
