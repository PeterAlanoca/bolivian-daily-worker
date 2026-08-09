using BolivianDaily.Shared.Entities;

namespace BolivianDaily.SyncWorker.Domain.Entities;

public class SyncedArticleMedia : IHasTimestamps
{
    public long Id { get; set; }
    public long SyncedArticleId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Path { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
