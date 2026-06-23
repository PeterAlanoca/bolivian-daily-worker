using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Domain.Entities;

public class ProcessedArticle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long SourceArticleId { get; set; }
    public long? CategoryId { get; set; }
    public long? SourceId { get; set; }
    public int UserId { get; set; } = 1;
    public string Title { get; set; } = string.Empty;
    public string? Pretitle { get; set; }
    public string? Subtitle { get; set; }
    public string? Enter { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string State { get; set; } = "A";
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string? WarningsJson { get; set; }
    public List<ProcessedArticleMedia> Multimedia { get; set; } = new();

    public ArticleProcessedEvent ToEvent()
    {
        return new ArticleProcessedEvent(
            SourceArticleId,
            Id,
            CategoryId,
            SourceId,
            UserId,
            Title,
            Pretitle,
            Subtitle,
            Enter,
            Body,
            Author,
            PublicationDate,
            State,
            Multimedia.Select(media => new ArticleMediaMessage(
                media.Url,
                media.Type,
                media.Description,
                media.Path)).ToArray(),
            ProcessedAt);
    }
}
