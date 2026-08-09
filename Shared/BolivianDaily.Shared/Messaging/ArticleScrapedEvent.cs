namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleScrapedEvent(
    long ArticleId,
    long? SourceId,
    string SourceName,
    string SourceUrl,
    string? Url,
    long? CategoryId,
    string? CategoryName,
    string? Pretitle,
    string Title,
    string? Subtitle,
    string? Lead,
    string? RawBody,
    string? Author,
    DateTime? PublishedAt,
    IReadOnlyCollection<ArticleMediaMessage> Media,
    DateTime ScrapedAt);
