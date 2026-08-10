using BolivianDaily.ScraperWorker.Application.Interfaces;

namespace BolivianDaily.ScraperWorker.Infrastructure.Parsers;

public class NewsSourceParserRegistry(IEnumerable<INewsSourceParser> newsSourceParsers) : INewsSourceParserRegistry
{
    private readonly IReadOnlyDictionary<string, INewsSourceParser> _newsSourceParsers = newsSourceParsers.ToDictionary(parser => parser.SourceAlias, StringComparer.OrdinalIgnoreCase);

    public bool HasParserFor(string sourceAlias) => _newsSourceParsers.ContainsKey(sourceAlias);

    public INewsSourceParser GetFor(string sourceAlias)
    {
        if (_newsSourceParsers.TryGetValue(sourceAlias, out var parser))
        {
            return parser;
        }

        throw new NotSupportedException($"No parser is registered for source alias '{sourceAlias}'.");
    }
}
