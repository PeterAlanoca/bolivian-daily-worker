namespace BolivianDaily.ScraperWorker.Application.Interfaces;

public interface INewsSourceParserRegistry
{
    bool HasParserFor(string sourceAlias);
    INewsSourceParser GetFor(string sourceAlias);
}
