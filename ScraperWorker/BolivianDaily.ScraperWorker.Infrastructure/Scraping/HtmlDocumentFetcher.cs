using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.ScraperWorker.Infrastructure.Scraping;

public class HtmlDocumentFetcher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HtmlDocumentFetcher> _logger;

    public HtmlDocumentFetcher(HttpClient httpClient, ILogger<HtmlDocumentFetcher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HtmlDocument?> FetchAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url, cancellationToken);
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch HTML document from {Url}", url);
            return null;
        }
    }
}
