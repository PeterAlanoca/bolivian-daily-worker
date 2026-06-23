namespace BolivianDaily.CheckerWorker.Domain.Entities;

public class ProcessedArticleMedia
{
    public long Id { get; set; }
    public Guid ProcessedArticleId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = "image/jpeg";
    public string? Description { get; set; }
    public string? Path { get; set; }
}
