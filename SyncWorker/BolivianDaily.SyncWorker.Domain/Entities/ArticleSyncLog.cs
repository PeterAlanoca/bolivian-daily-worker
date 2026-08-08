namespace BolivianDaily.SyncWorker.Domain.Entities;

public class ArticleSyncLog
{
    public long Id { get; set; }
    public long CheckedArticleId { get; set; }
    public long ScrapedArticleId { get; set; }
    public string Status { get; set; } = "Pending";
    public int Attempts { get; set; }
    public string? ExternalId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }
}
