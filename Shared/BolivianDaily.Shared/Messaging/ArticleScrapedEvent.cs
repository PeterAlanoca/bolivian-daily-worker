namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleScrapedEvent(
    long ArticleId,
    long? SourceId,
    string SourceName,
    string SourceUrl,
    long? CategoryId,
    string? CategoryName,
    string? Pretitle,
    string Title,
    string? Subtitle,
    string? Lead,
    string? RawBody,
    string? Author,
    DateTime? PublicationDate,
    IReadOnlyCollection<ArticleMediaMessage> Multimedia,
    DateTime ScrapedAt);
