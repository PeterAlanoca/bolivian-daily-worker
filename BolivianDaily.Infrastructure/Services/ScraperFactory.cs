namespace BolivianDaily.Infrastructure.Services;

using BolivianDaily.Application.Interfaces;
using BolivianDaily.Infrastructure.Services.Scrapers;

/// <summary>
/// Factory that resolves the correct portal-specific IWebScraperService based on the Source alias.
/// To add a new portal: create a new XxxScraperService class and add a new entry to _scrapers.
/// </summary>
public class ScraperFactory : IScraperFactory
{
    private readonly Dictionary<string, IWebScraperService> _scrapers;

    public ScraperFactory(
        JornadaScraperService jornadaScraper,
        ElDeberScraperService elDeberScraper)
    {
        _scrapers = new Dictionary<string, IWebScraperService>(StringComparer.OrdinalIgnoreCase)
        {
            { "jornada", jornadaScraper },
            { "el_deber", elDeberScraper }
        };
    }

    /// <summary>
    /// Returns the specific scraper for a given source alias.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when no scraper is registered for the given alias.</exception>
    public IWebScraperService GetFor(string alias)
    {
        if (_scrapers.TryGetValue(alias, out var scraper))
            return scraper;

        throw new NotSupportedException($"No scraper is registered for portal alias '{alias}'. " +
                                        $"Please implement a new IWebScraperService and register it in ScraperFactory.");
    }

    public bool HasScraperFor(string alias) => _scrapers.ContainsKey(alias);
}
