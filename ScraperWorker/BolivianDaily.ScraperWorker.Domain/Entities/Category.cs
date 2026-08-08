using BolivianDaily.Shared.Entities;

namespace BolivianDaily.ScraperWorker.Domain.Entities;

public class Category : IHasTimestamps
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string State { get; set; } = "A";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
