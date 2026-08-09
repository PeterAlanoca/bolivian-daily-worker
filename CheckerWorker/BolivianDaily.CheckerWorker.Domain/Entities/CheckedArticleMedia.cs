using BolivianDaily.Shared.Entities;

namespace BolivianDaily.CheckerWorker.Domain.Entities;

public class CheckedArticleMedia : IHasTimestamps
{
    public long Id { get; set; }
    public long CheckedArticleId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = "image/jpeg";
    public string? Description { get; set; }
    public string? Path { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
