using BolivianDaily.Shared.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Entities;

public class Article : IHasTimestamps
{
    public long Id { get; set; }
    public long? NewsSourceId { get; set; }
    public long? CategoryId { get; set; }
    public long? SourceCategoryId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Pretitle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Lead { get; set; }
    public string? Body { get; set; }
    public string? Author { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
    public string State { get; set; } = "A";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public NewsSource? NewsSource { get; set; }
    public Category? Category { get; set; }
    public SourceCategory? SourceCategory { get; set; }
    public List<ArticleMedia> Media { get; set; } = new();
}
