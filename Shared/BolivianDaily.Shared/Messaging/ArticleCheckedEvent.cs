namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleCheckedEvent(
    long ScrapedArticleId,
    long CheckedArticleId,
    long? CategoryId,
    long? SourceId,
    string SourceName,
    string SourceUrl,
    string? Url,
    string? CategoryName,
    string Title,
    string? Pretitle,
    string? Subtitle,
    string? Lead,
    string Body,
    string? Author,
    DateTime? PublishedAt,
    DateTime ScrapedAt,
    IReadOnlyCollection<ArticleMediaMessage> Media,
    DateTime CheckedAt);
