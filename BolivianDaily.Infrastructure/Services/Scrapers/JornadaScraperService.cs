namespace BolivianDaily.Infrastructure.Services.Scrapers;

using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;

/// <summary>
/// Scraper specific to the Jornada news portal (jornada.com.bo).
/// HTML structure based on TDBuilder WordPress theme.
/// </summary>
public class JornadaScraperService : BaseScraperService
{
    // CSS/XPath selectors specific to jornada.com.bo
    private const string NewsLinkSelector = "//div[contains(@class,'td_module')]//a[contains(@class,'td-image-wrap') or @rel='bookmark']";
    private const string TitleSelector = "//h1[contains(@class,'entry-title') or contains(@class,'tdb-title-text')]";
    private const string PretitleSelector = "//div[contains(@class,'td-post-above-title')]";
    private const string SubtitleSelector = "//div[contains(@class,'td-post-below-title')]";
    private const string BodySelector = "//div[@class='td-post-content' or contains(@class,'tdb-block-inner')]//p";
    private const string AuthorSelector = "//span[contains(@class,'td-post-author-name')]//a";
    private const string ImageSelector = "//div[contains(@class,'td-post-content')]//img";

    public JornadaScraperService(HttpClient httpClient, ILogger<JornadaScraperService> logger)
        : base(httpClient, logger)
    {
    }

    public override async Task<List<string>> GetLatestArticleUrlsAsync(Source source, Category category, CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();

        if (string.IsNullOrEmpty(category.Url)) return urls;

        var document = await FetchDocumentAsync(category.Url, cancellationToken);
        if (document == null) return urls;

        var linkNodes = document.DocumentNode.SelectNodes(NewsLinkSelector);
        if (linkNodes == null) return urls;

        foreach (var node in linkNodes)
        {
            var href = node.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href)) continue;

            var fullUrl = BuildFullUrl(href, source.Url);
            if (!urls.Contains(fullUrl)) urls.Add(fullUrl);
        }

        Logger.LogInformation("[Jornada] Found {Count} article URLs in category '{Category}'", urls.Count, category.Name);
        return urls;
    }

    public override async Task<News?> ScrapeArticleAsync(Source source, string articleUrl, CancellationToken cancellationToken = default)
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

            Logger.LogInformation("[Jornada] Scraped article: {Title}", newsArticle.Title);
            return newsArticle;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Jornada] Failed to parse article at {Url}", articleUrl);
            return null;
        }
    }
}
