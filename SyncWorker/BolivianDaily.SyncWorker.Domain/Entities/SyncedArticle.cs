using BolivianDaily.Shared.Entities;

namespace BolivianDaily.SyncWorker.Domain.Entities;

public class SyncedArticle : IHasTimestamps
{
    public long Id { get; set; }
    public long CheckedArticleId { get; set; }
    public long ScrapedArticleId { get; set; }
    public long? ExtranetId { get; set; }
    public long? SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ExtranetUrl { get; set; }
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Pretitle { get; set; }
    public string? Subtitle { get; set; }
    public string? Lead { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime ScrapedAt { get; set; }
    public DateTime CheckedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public string? Details { get; set; }
    public DateTime? SyncedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SyncedArticleMedia> Media { get; set; } = new();
}
