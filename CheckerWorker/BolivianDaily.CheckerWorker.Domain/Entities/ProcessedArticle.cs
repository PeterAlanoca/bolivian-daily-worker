using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Domain.Entities;

public class ProcessedArticle
{
    public long Id { get; set; }
    public long ScrapedArticleId { get; set; }
    public long? SourceId { get; set; }
    public long? CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Pretitle { get; set; }
    public string? Subtitle { get; set; }
    public string? Enter { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string State { get; set; } = "A";
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string? Warnings { get; set; }

    public bool IsValid { get; set; }
    public bool HtmlFormatPassed { get; set; }
    public string? HtmlFormatReason { get; set; }
    public bool CategoryAccuracyPassed { get; set; }
    public string? CategoryAccuracyReason { get; set; }
    public string? SuggestedCategory { get; set; }
    public bool NoAdvertisingPassed { get; set; }
    public string? NoAdvertisingReason { get; set; }
    public string? DetectedNetworksJson { get; set; }
    public bool ReadyToPublishPassed { get; set; }
    public string? ReadyToPublishReason { get; set; }

    public List<ProcessedArticleMedia> Multimedia { get; set; } = new();

    public ArticleProcessedEvent ToEvent()
    {
        return new ArticleProcessedEvent(
            ScrapedArticleId,
            Id,
            CategoryId,
            SourceId,
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
