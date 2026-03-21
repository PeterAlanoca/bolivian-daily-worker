namespace BolivianDaily.Application.Interfaces;

/// <summary>
/// Factory abstraction to decouple the Application layer from Infrastructure implementations.
/// The Application layer works with this interface, unaware of which concrete scraper is used.
/// </summary>
public interface IScraperFactory
{
    IWebScraperService GetFor(string alias);
    bool HasScraperFor(string alias);
}
