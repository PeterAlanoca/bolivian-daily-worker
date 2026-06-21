using BolivianDaily.ScraperWorker.Application.Interfaces;

namespace BolivianDaily.ScraperWorker.Infrastructure.Parsers;

public class NewsSourceParserRegistry(IEnumerable<INewsSourceParser> parsers) : INewsSourceParserRegistry
{
    private readonly IReadOnlyDictionary<string, INewsSourceParser> _parsers = parsers.ToDictionary(parser => parser.SourceAlias, StringComparer.OrdinalIgnoreCase);

    public bool HasParserFor(string sourceAlias) => _parsers.ContainsKey(sourceAlias);

    public INewsSourceParser GetFor(string sourceAlias)
    {
        if (_parsers.TryGetValue(sourceAlias, out var parser))
        {
            return parser;
        }

        throw new NotSupportedException($"No parser is registered for source alias '{sourceAlias}'.");
    }
}
