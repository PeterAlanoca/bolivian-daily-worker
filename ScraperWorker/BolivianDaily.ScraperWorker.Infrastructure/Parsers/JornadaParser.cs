using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Infrastructure.Scraping;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.ScraperWorker.Infrastructure.Parsers;

public class JornadaParser : INewsSourceParser
{
    private const string NewsLinkSelector = "//div[contains(@class,'td-module') or contains(@class,'tdb_module')][.//a[contains(@class,'td-post-category')]]//a[(@rel='bookmark' or contains(@class,'td-image-wrap')) and not(contains(@class,'td-post-category'))]";
    private const string TitleSelector = "//h1[contains(@class,'entry-title') or contains(@class,'tdb-title-text')]";
    private const string PretitleSelector = "//div[contains(@class,'td-post-above-title')]";
    private const string SubtitleSelector = "//div[contains(@class,'td-post-below-title')]";
    private const string BodySelector = "//div[contains(@class, 'tdb_single_content')]//div[contains(@class, 'tdb-block-inner')]";
    private const string AuthorSelector = "//a[contains(@class,'tdb-author-name')]";
    private const string ImageSelector = "//img[contains(@class,'td-modal-image')]";

    private readonly HtmlDocumentFetcher _fetcher;
    private readonly ILogger<JornadaParser> _logger;

    public JornadaParser(HtmlDocumentFetcher fetcher, ILogger<JornadaParser> logger)
    {
        _fetcher = fetcher;
        _logger = logger;
    }

    public string SourceAlias => "jornada";

    public async Task<IReadOnlyCollection<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();
        if (string.IsNullOrWhiteSpace(sourceCategory.Url))
        {
            return urls;
        }

        var document = await _fetcher.FetchAsync(sourceCategory.Url, cancellationToken);
        if (document is null)
        {
            return urls;
        }

        var linkNodes = document.DocumentNode.SelectNodes(NewsLinkSelector)
            ?? document.DocumentNode.SelectNodes("//h2//a[@href] | //h3//a[@href] | //article//a[@href]");

        if (linkNodes is null)
        {
            return urls;
        }

        foreach (var node in linkNodes)
        {
            var href = node.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href))
            {
                continue;
            }

            var fullUrl = BuildFullUrl(href, sourceCategory.NewsSource?.BaseUrl ?? sourceCategory.Url);
            if (IsArticleUrl(fullUrl) && !urls.Contains(fullUrl))
            {
                urls.Add(fullUrl);
            }
        }

        _logger.LogInformation("[Jornada] Found {Count} article URLs in category {Category}", urls.Count, sourceCategory.Name);
        return urls;
    }

    public async Task<Article?> ParseArticleAsync(string articleUrl, CancellationToken cancellationToken = default)
    {
        var document = await _fetcher.FetchAsync(articleUrl, cancellationToken);
        if (document is null)
        {
            return null;
        }

        try
        {
            var titleNode = document.DocumentNode.SelectSingleNode(TitleSelector) ?? document.DocumentNode.SelectSingleNode("//h1");
            var pretitleNode = document.DocumentNode.SelectSingleNode(PretitleSelector);
            var subtitleNode = document.DocumentNode.SelectSingleNode(SubtitleSelector);
            var bodyNode = document.DocumentNode.SelectSingleNode(BodySelector)
                ?? document.DocumentNode.SelectSingleNode("//article")
                ?? document.DocumentNode.SelectSingleNode("//div[contains(@class, 'td-post-content')]");
            var authorNode = document.DocumentNode.SelectSingleNode(AuthorSelector)
                ?? document.DocumentNode.SelectSingleNode("//meta[@name='author']");

            var article = new Article
            {
                Url = articleUrl,
                Title = CleanText(titleNode?.InnerText) ?? "Without Title",
                Pretitle = CleanText(pretitleNode?.InnerText),
                Subtitle = CleanText(subtitleNode?.InnerText),
                Author = CleanText(authorNode?.InnerText) ?? CleanText(authorNode?.GetAttributeValue("content", string.Empty)),
                PublishedAt = ExtractPublicationDate(document),
                ScrapedAt = DateTime.UtcNow,
                State = "A"
            };

            if (bodyNode is not null)
            {
                RemoveJunkNodes(bodyNode);

                var paragraphs = bodyNode.SelectNodes(".//p")
                    ?.Select(p => CleanText(p.InnerText))
                    .Where(text => !string.IsNullOrWhiteSpace(text) && text.Length > 20)
                    .Cast<string>()
                    .ToList();

                if (paragraphs is { Count: > 0 })
                {
                    article.Lead = paragraphs[0];
                    article.Body = string.Join(Environment.NewLine + Environment.NewLine, paragraphs.Skip(1));
                }
                else
                {
                    article.Body = CleanText(bodyNode.InnerText);
                }
            }

            article.Media = ExtractMedia(document);
            return article;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Jornada] Failed to parse article at {Url}", articleUrl);
            return null;
        }
    }

    private static bool IsArticleUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var path = uri.AbsolutePath.Trim('/');
        return uri.Host.Contains("jornada.com.bo", StringComparison.OrdinalIgnoreCase)
            && path.Length > 0
            && !path.StartsWith("seccion/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("tag/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("author/", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("page/", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildFullUrl(string href, string baseUrl)
    {
        if (Uri.TryCreate(href, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        return Uri.TryCreate(new Uri(baseUrl), href, out var fullUri)
            ? fullUri.ToString()
            : href;
    }

    private static DateTime? ExtractPublicationDate(HtmlDocument document)
    {
        var dateNode = document.DocumentNode.SelectSingleNode("//meta[@property='article:published_time']")
            ?? document.DocumentNode.SelectSingleNode("//meta[@property='article:modified_time']");

        var dateText = dateNode?.GetAttributeValue("content", string.Empty);
        if (DateTimeOffset.TryParse(dateText, out var date))
        {
            return date.UtcDateTime;
        }

        return null;
    }

    private static List<ArticleMedia> ExtractMedia(HtmlDocument document)
    {
        var media = new List<ArticleMedia>();
        var imgNode = document.DocumentNode.SelectSingleNode(ImageSelector)
            ?? document.DocumentNode.SelectSingleNode("//meta[@property='og:image']")
            ?? document.DocumentNode.SelectSingleNode("//article//img");

        if (imgNode is null)
        {
            return media;
        }

        var src = imgNode.Name.Equals("meta", StringComparison.OrdinalIgnoreCase)
            ? imgNode.GetAttributeValue("content", string.Empty)
            : imgNode.GetAttributeValue("src", string.Empty);

        if (string.IsNullOrWhiteSpace(src))
        {
            return media;
        }

        var caption = CleanText(document.DocumentNode.SelectSingleNode("//figcaption[contains(@class, 'tdb-caption-text')]")?.InnerText)
            ?? CleanText(imgNode.GetAttributeValue("alt", string.Empty));

        media.Add(new ArticleMedia
        {
            Url = src,
            Description = caption,
            Type = GetMimeType(src),
            State = "A"
        });

        return media;
    }

    private static void RemoveJunkNodes(HtmlNode bodyNode)
    {
        var junkNodes = bodyNode.SelectNodes(".//div[contains(@class, 'td-a-ad')] | .//script | .//ins | .//style | .//iframe");
        if (junkNodes is null)
        {
            return;
        }

        foreach (var junk in junkNodes)
        {
            junk.Remove();
        }
    }

    private static string? CleanText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return HtmlEntity.DeEntitize(value).Trim();
    }

    private static string GetMimeType(string url)
    {
        var extension = Path.GetExtension(url).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/jpeg"
        };
    }
}
