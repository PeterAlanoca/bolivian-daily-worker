namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleProcessedEvent(
    long SourceArticleId,
    Guid ProcessedArticleId,
    long? CategoryId,
    long? SourceId,
    int UserId,
    string Title,
    string? Pretitle,
    string? Subtitle,
    string? Enter,
    string Body,
    string? Author,
    DateTime? PublicationDate,
    string State,
    IReadOnlyCollection<ArticleMediaMessage> Multimedia,
    DateTime ProcessedAt);
