namespace BolivianDaily.ScraperWorker.Infrastructure.Configuration;

public class NewsSourceOptions
{
    public required string Alias { get; set; }
    public int IntervalMinutes { get; set; }
    public int CategoryDelayMs { get; set; }
    public int MinArticleDelayMs { get; set; }
    public int MaxArticleDelayMs { get; set; }
}