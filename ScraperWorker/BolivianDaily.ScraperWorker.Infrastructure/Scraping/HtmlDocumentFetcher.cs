using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.ScraperWorker.Infrastructure.Scraping;

public class HtmlDocumentFetcher(HttpClient httpClient, ILogger<HtmlDocumentFetcher> logger)
{
    public async Task<HtmlDocument?> FetchAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await httpClient.GetStringAsync(url, cancellationToken);
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch HTML document from {Url}", url);
            return null;
        }
    }
}
