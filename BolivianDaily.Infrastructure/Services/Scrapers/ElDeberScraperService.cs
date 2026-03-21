namespace BolivianDaily.Infrastructure.Services.Scrapers;

using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;

/// <summary>
/// Scraper specific to El Deber news portal (eldeber.com.bo).
/// HTML structure based on their custom CMS.
/// </summary>
public class ElDeberScraperService : BaseScraperService
{
    // CSS/XPath selectors specific to eldeber.com.bo
    private const string NewsLinkSelector = "//article//a[contains(@class,'single-link') or @class='']";
    private const string TitleSelector = "//h1[contains(@class,'single-title') or @class='title']";
    private const string PretitleSelector = "//span[contains(@class,'supra-title') or contains(@class,'kicker')]";
    private const string SubtitleSelector = "//div[contains(@class,'summary') or contains(@class,'subtitle')]";
    private const string BodySelector = "//div[contains(@class,'body-item') or contains(@class,'article-body')]//p";
    private const string AuthorSelector = "//a[contains(@class,'author-name')] | //span[contains(@class,'author')]";
    private const string ImageSelector = "//figure[contains(@class,'article')]//img | //div[contains(@class,'body-item')]//img";
    private const string DateSelector = "//meta[@property='article:published_time'] | //time[@class='date']";

    public ElDeberScraperService(HttpClient httpClient, ILogger<ElDeberScraperService> logger)
        : base(httpClient, logger)
    {
    }

    public override async Task<List<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();

        if (string.IsNullOrEmpty(sourceCategory.Url)) return urls;

        var document = await FetchDocumentAsync(sourceCategory.Url, cancellationToken);
        if (document == null) return urls;

        // El Deber uses <article> tags wrapping each news card
        var articleNodes = document.DocumentNode.SelectNodes("//article//a");
        if (articleNodes == null) return urls;

        foreach (var node in articleNodes)
        {
            var href = node.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href)) continue;

            // El Deber articles typically have paths like /economia/noticia-xxx
            if (!href.Contains("/noticia") && !href.Contains("/articulo")) continue;

            var fullUrl = BuildFullUrl(href, sourceCategory.Source?.Url ?? string.Empty);
            if (!urls.Contains(fullUrl)) urls.Add(fullUrl);
        }

        Logger.LogInformation("[ElDeber] Found {Count} article URLs in category '{Category}'", urls.Count, sourceCategory.Category?.Name);
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
            var bodyNodes = document.DocumentNode.SelectNodes(BodySelector);
            var authorNode = document.DocumentNode.SelectSingleNode(AuthorSelector);
            
            // El Deber stores the date in a <time> element with a datetime attribute
            var timeNode = document.DocumentNode.SelectSingleNode("//time[@datetime]");
            var timeStr = timeNode?.GetAttributeValue("datetime", string.Empty);
            
            DateTime? publicationDate = null;
            if (DateTime.TryParse(timeStr, out var parsedDate))
            {
                publicationDate = parsedDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc)
                    : parsedDate.ToUniversalTime();
            }
            else
            {
                publicationDate = ExtractPublicationDate(document);
            }

            var newsArticle = new News
            {
                Url = articleUrl,
                Title = titleNode?.InnerText.Trim() ?? "Without Title",
                Pretitle = pretitleNode?.InnerText.Trim(),
                Subtitle = subtitleNode?.InnerText.Trim(),
                Author = authorNode?.InnerText.Trim(),
                PublicationDate = publicationDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (bodyNodes != null)
            {
                var paragraphs = bodyNodes
                    .Select(n => n.InnerText.Trim())
                    .Where(t => t.Length > 20)
                    .ToList();

                newsArticle.Enter = paragraphs.FirstOrDefault();
                newsArticle.Body = string.Join("\n\n", paragraphs);
            }

            newsArticle.Multimedia = ExtractMultimedia(document, ImageSelector);

            Logger.LogInformation("[ElDeber] Scraped article: {Title}", newsArticle.Title);
            return newsArticle;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[ElDeber] Failed to parse article at {Url}", articleUrl);
            return null;
        }
    }
}
