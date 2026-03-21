namespace BolivianDaily.Infrastructure.Services.Scrapers;

using HtmlAgilityPack;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;

/// <summary>
/// Base class with shared utilities for all portal-specific scrapers.
/// Each concrete scraper inherits from this and overrides the scraping logic.
/// </summary>
public abstract class BaseScraperService : IWebScraperService
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;

    protected BaseScraperService(HttpClient httpClient, ILogger logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    public abstract Task<List<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default);
    public abstract Task<News?> ScrapeArticleAsync(string articleUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads and parses an HTML page into an HtmlDocument.
    /// </summary>
    protected async Task<HtmlDocument?> FetchDocumentAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var html = await HttpClient.GetStringAsync(url, cancellationToken);
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fetch HTML document from {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// Extracts and normalizes image URLs from img nodes, populating the Multimedia list.
    /// Default implementation takes up to 5 images from the provided XPath.
    /// </summary>
    protected virtual List<Multimedia> ExtractMultimedia(HtmlDocument document, string imgXPath)
    {
        var mediaList = new List<Multimedia>();
        var imgNodes = document.DocumentNode.SelectNodes(imgXPath);
        if (imgNodes == null) return mediaList;

        foreach (var img in imgNodes.Take(5))
        {
            var src = img.GetAttributeValue("src", string.Empty);
            if (!string.IsNullOrEmpty(src) && src.StartsWith("http"))
            {
                mediaList.Add(new Multimedia
                {
                    Type = "image",
                    Url = src,
                    Description = img.GetAttributeValue("alt", string.Empty),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        return mediaList;
    }

    /// <summary>
    /// Extracts publication date from an HTML meta tag (Open Graph standard).
    /// Falls back to current UTC time if not found.
    /// </summary>
    protected DateTime? ExtractPublicationDate(HtmlDocument document)
    {
        var metaNode = document.DocumentNode
            .SelectSingleNode("//meta[@property='article:published_time']");

        var dateStr = metaNode?.GetAttributeValue("content", string.Empty);
        if (DateTime.TryParse(dateStr, out var result))
        {
            return result;
        }

        return DateTime.UtcNow;
    }

    /// <summary>
    /// Builds a full absolute URL from a relative href and a base URL.
    /// </summary>
    protected string BuildFullUrl(string href, string baseUrl)
    {
        if (href.StartsWith("http")) return href;
        return $"{baseUrl.TrimEnd('/')}/{href.TrimStart('/')}";
    }
}
