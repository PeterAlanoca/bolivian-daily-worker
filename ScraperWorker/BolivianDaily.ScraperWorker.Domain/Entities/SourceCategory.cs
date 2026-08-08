using BolivianDaily.Shared.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Entities;

public class SourceCategory : IHasTimestamps
{
    public long Id { get; set; }
    public long NewsSourceId { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string State { get; set; } = "A";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public NewsSource? NewsSource { get; set; }
    public Category? Category { get; set; }
}
