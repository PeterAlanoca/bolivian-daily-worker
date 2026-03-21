namespace BolivianDaily.Infrastructure.Services.Scrapers;

using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using HtmlAgilityPack;

/// <summary>
/// Scraper specific to the Jornada news portal (jornada.com.bo).
/// HTML structure based on TDBuilder WordPress theme.
/// </summary>
public class JornadaScraperService : BaseScraperService
{
    // CSS/XPath selectors specific to jornada.com.bo
    private const string NewsLinkSelector = "//div[contains(@class,'td-module') or contains(@class,'tdb_module')][.//a[contains(@class,'td-post-category')]]//a[(@rel='bookmark' or contains(@class,'td-image-wrap')) and not(contains(@class,'td-post-category'))]";
    private const string TitleSelector = "//h1[contains(@class,'entry-title') or contains(@class,'tdb-title-text')]";
    private const string PretitleSelector = "//div[contains(@class,'td-post-above-title')]";
    private const string SubtitleSelector = "//div[contains(@class,'td-post-below-title')]";
    private const string BodySelector = "//div[contains(@class, 'tdb_single_content')]//div[contains(@class, 'tdb-block-inner')]";
    private const string AuthorSelector = "//a[contains(@class,'tdb-author-name')]";
    private const string ImageSelector = "//img[contains(@class,'td-modal-image')]";

    public JornadaScraperService(HttpClient httpClient, ILogger<JornadaScraperService> logger)
        : base(httpClient, logger)
    {
    }

    public override async Task<List<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();

        if (string.IsNullOrEmpty(sourceCategory.Url)) return urls;

        var document = await FetchDocumentAsync(sourceCategory.Url, cancellationToken);
        if (document == null) return urls;

        var linkNodes = document.DocumentNode.SelectNodes(NewsLinkSelector);
        if (linkNodes == null) return urls;

        foreach (var node in linkNodes)
        {
            var href = node.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href)) continue;

            var fullUrl = BuildFullUrl(href, sourceCategory.Source?.Url ?? string.Empty);
            if (!urls.Contains(fullUrl)) urls.Add(fullUrl);
        }

        Logger.LogInformation("[Jornada] Found {Count} article URLs in category '{Category}'", urls.Count, sourceCategory.Category?.Name);
        return urls;
    }

    public override async Task<News?> ScrapeArticleAsync(string articleUrl, CancellationToken cancellationToken = default)
    {
        var document = await FetchDocumentAsync(articleUrl, cancellationToken);
        if (document == null) return null;

        try
        {
            var titleNode = document.DocumentNode.SelectSingleNode(TitleSelector);
            var pretitleNode = document.DocumentNode.SelectSingleNode(PretitleSelector);
            var subtitleNode = document.DocumentNode.SelectSingleNode(SubtitleSelector);
            var bodyNode = document.DocumentNode.SelectSingleNode(BodySelector);
            var authorNode = document.DocumentNode.SelectSingleNode(AuthorSelector);

            var newsArticle = new News
            {
                Url = articleUrl,
                Title = titleNode?.InnerText.Trim() ?? "Without Title",
                Pretitle = pretitleNode?.InnerText.Trim(),
                Subtitle = subtitleNode?.InnerText.Trim(),
                Author = authorNode?.InnerText.Trim(),
                PublicationDate = ExtractPublicationDate(document),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (bodyNode != null)
            {
                // Clean ads, scripts and styles to keep only content
                var junkNodes = bodyNode.SelectNodes(".//div[contains(@class, 'td-a-ad')] | .//script | .//ins | .//style | .//iframe");
                if (junkNodes != null)
                {
                    foreach (var junk in junkNodes)
                    {
                        junk.Remove();
                    }
                }

                // Get all paragraphs
                var paragraphs = bodyNode.SelectNodes(".//p");
                if (paragraphs != null && paragraphs.Count > 0)
                {
                    // The first paragraph is the Enter (Lead)
                    newsArticle.Enter = paragraphs[0].InnerText.Trim();

                    // The rest of the paragraphs form the Body
                    var bodyParagraphs = paragraphs.Skip(1).Select(p => p.OuterHtml);
                    newsArticle.Body = string.Join("", bodyParagraphs).Trim();
                }
                else
                {
                    // Fallback if no paragraphs are found
                    newsArticle.Body = bodyNode.InnerHtml.Trim();
                }
            }

            newsArticle.Multimedia = ExtractMultimedia(document, ImageSelector);

            Logger.LogInformation("[Jornada] Scraped article: {Title}", newsArticle.Title);
            return newsArticle;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Jornada] Failed to parse article at {Url}", articleUrl);
            return null;
        }
    }

    protected override List<Multimedia> ExtractMultimedia(HtmlDocument document, string imgXPath)
    {
        var mediaList = new List<Multimedia>();
        var imgNode = document.DocumentNode.SelectSingleNode(imgXPath);
        
        if (imgNode != null)
        {
            var src = imgNode.GetAttributeValue("src", string.Empty);
            if (!string.IsNullOrEmpty(src))
            {
                // Try to find the caption in figcaption.tdb-caption-text
                // Usually it's a sibling of the parent <a> or child of the parent <figure>
                var caption = document.DocumentNode.SelectSingleNode("//figcaption[contains(@class, 'tdb-caption-text')]")?.InnerText?.Trim();
                
                // If not found, fall back to alt text
                if (string.IsNullOrEmpty(caption))
                {
                    caption = imgNode.GetAttributeValue("alt", string.Empty);
                }

                mediaList.Add(new Multimedia
                {
                    Type = GetMimeType(src),
                    Url = src,
                    Description = caption,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        return mediaList;
    }

    private string GetMimeType(string url)
    {
        var extension = System.IO.Path.GetExtension(url).ToLower();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/jpeg" // Default fallback
        };
    }
}
