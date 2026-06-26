namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleProcessedEvent(
    long ScrapedArticleId,
    long ProcessedArticleId,
    long? CategoryId,
    long? SourceId,
    string SourceName,
    string SourceUrl,
    string? ArticleUrl,
    string? CategoryName,
    string Title,
    string? Pretitle,
    string? Subtitle,
    string? Enter,
    string Body,
    string? Author,
    DateTime? PublicationDate,
    DateTime ScrapedAt,
    string State,
    IReadOnlyCollection<ArticleMediaMessage> Multimedia,
    DateTime ProcessedAt);
