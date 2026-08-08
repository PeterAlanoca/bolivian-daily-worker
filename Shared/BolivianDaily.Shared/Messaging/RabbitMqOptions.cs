namespace BolivianDaily.Shared.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Exchange { get; set; } = "bolivian-daily.articles";
    public string ArticleScrapedQueue { get; set; } = "checker.article-scraped";
    public string ArticleCheckedQueue { get; set; } = "sync.article-checked";
    public string ArticleScrapedRoutingKey { get; set; } = "article.scraped";
    public string ArticleCheckedRoutingKey { get; set; } = "article.checked";
    public int MaxRetries { get; set; } = 3;
}
