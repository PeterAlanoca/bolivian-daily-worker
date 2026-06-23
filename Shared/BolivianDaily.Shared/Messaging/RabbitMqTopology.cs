namespace BolivianDaily.Shared.Messaging;

public static class RabbitMqTopology
{
    public static string ArticleScrapedRoutingKey(RabbitMqOptions options) => options.ArticleScrapedRoutingKey;

    public static string ArticleProcessedRoutingKey(RabbitMqOptions options) => options.ArticleProcessedRoutingKey;
}
