namespace BolivianDaily.Shared.Messaging;

public sealed record ArticleMediaMessage(
    string Url,
    string Type,
    string? Description = null,
    string? Path = null);
