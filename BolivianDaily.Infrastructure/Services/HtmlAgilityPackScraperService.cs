namespace BolivianDaily.Infrastructure.Services;

using HtmlAgilityPack;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;

public class HtmlAgilityPackScraperService : IWebScraperService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HtmlAgilityPackScraperService> _logger;

    public HtmlAgilityPackScraperService(HttpClient httpClient, ILogger<HtmlAgilityPackScraperService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<string>> GetLatestArticleUrlsAsync(SourceCategory sourceCategory, CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();
        try
        {
            if (string.IsNullOrEmpty(sourceCategory.Url)) return urls;

            var html = await _httpClient.GetStringAsync(sourceCategory.Url, cancellationToken);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            // Using placeholder logic. Normally this maps to custom Source selectors 
            var linkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes != null)
            {
                foreach (var node in linkNodes.Take(15))
                {
                    var href = node.GetAttributeValue("href", string.Empty);
                    if (href.Contains("noticia") || href.Length > 40)
                    {
                        var fullUrl = href.StartsWith("http") ? href : $"{sourceCategory.Source!.Url!.TrimEnd('/')}/{href.TrimStart('/')}";
                        if (!urls.Contains(fullUrl)) urls.Add(fullUrl);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to fetch article URLs from {sourceCategory.Url}");
        }

        return urls;
    }

    public async Task<News?> ScrapeArticleAsync(string articleUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(articleUrl, cancellationToken);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//h1");
            var bodyNodes = htmlDocument.DocumentNode.SelectNodes("//p");
            var imgNodes = htmlDocument.DocumentNode.SelectNodes("//article//img") ?? htmlDocument.DocumentNode.SelectNodes("//img");

            var newsArticle = new News
            {
                Title = titleNode?.InnerText.Trim() ?? "Without Title",
                Url = articleUrl,
                PublicationDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (bodyNodes != null)
            {
                var bodyText = string.Join("\n", bodyNodes.Select(n => n.InnerText.Trim()).Where(s => s.Length > 20));
                newsArticle.Body = bodyText;
                newsArticle.Enter = bodyNodes.FirstOrDefault(n => n.InnerText.Length > 20)?.InnerText.Trim();
            }

            if (imgNodes != null)
            {
                foreach (var img in imgNodes.Take(5)) // limit external multimedia extraction
                {
                    var src = img.GetAttributeValue("src", string.Empty);
                    if (!string.IsNullOrEmpty(src) && src.StartsWith("http"))
                    {
                        newsArticle.Multimedia.Add(new Multimedia
                        {
                            Type = "image",
                            Url = src,
                            Description = img.GetAttributeValue("alt", string.Empty),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            return newsArticle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to scrape article content from {articleUrl}");
            return null;
        }
    }
}
