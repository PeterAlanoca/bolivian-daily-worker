namespace BolivianDaily.SyncWorker.Domain.Entities;

public class ArticleSyncLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProcessedArticleId { get; set; }
    public long SourceArticleId { get; set; }
    public string Status { get; set; } = "Pending";
    public int Attempts { get; set; }
    public string? ExternalId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }
}
