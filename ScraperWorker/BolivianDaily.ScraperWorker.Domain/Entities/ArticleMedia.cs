using BolivianDaily.Shared.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Entities;

public class ArticleMedia : IHasTimestamps
{
    public long Id { get; set; }
    public long? ArticleId { get; set; }
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string Type { get; set; } = "image/jpeg";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
