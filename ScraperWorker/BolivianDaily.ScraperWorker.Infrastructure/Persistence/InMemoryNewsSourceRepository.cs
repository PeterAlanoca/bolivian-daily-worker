using BolivianDaily.ScraperWorker.Domain.Entities;
using BolivianDaily.ScraperWorker.Domain.Repositories;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class InMemoryNewsSourceRepository : INewsSourceRepository
{
    private readonly IReadOnlyCollection<NewsSource> _sources;

    public InMemoryNewsSourceRepository()
    {
        var nacional = new Category { Id = 1, Name = "NACIONAL", Slug = "nacional" };
        var economia = new Category { Id = 2, Name = "ECONOMIA", Slug = "economia" };
        var internacional = new Category { Id = 3, Name = "INTERNACIONAL", Slug = "internacional" };
        var deportes = new Category { Id = 8, Name = "DEPORTES", Slug = "deportes" };
        var salud = new Category { Id = 9, Name = "SALUD", Slug = "salud" };
        var interesante = new Category { Id = 10, Name = "INTERESANTE", Slug = "interesante" };
        var tecnologia = new Category { Id = 7, Name = "TECNOLOGIA", Slug = "tecnologia" };

        var jornada = new NewsSource
        {
            Id = 1,
            Name = "Jornada",
            Alias = "jornada",
            BaseUrl = "https://jornada.com.bo/",
            State = "A"
        };

        jornada.Categories.AddRange(new[]
        {
            CreateSourceCategory(1, jornada, nacional, "BOLIVIA", "https://jornada.com.bo/seccion/bolivia/"),
            CreateSourceCategory(2, jornada, economia, "ECONOMIA", "https://jornada.com.bo/seccion/economia/"),
            CreateSourceCategory(3, jornada, internacional, "MUNDO", "https://jornada.com.bo/seccion/mundo/"),
            CreateSourceCategory(4, jornada, deportes, "DEPORTES", "https://jornada.com.bo/seccion/deportes/"),
            CreateSourceCategory(5, jornada, interesante, "GENTE", "https://jornada.com.bo/seccion/gente/"),
            CreateSourceCategory(6, jornada, salud, "SALUD", "https://jornada.com.bo/seccion/salud/"),
            CreateSourceCategory(7, jornada, tecnologia, "TECNOLOGIA", "https://jornada.com.bo/seccion/tecnologia/")
        });

        _sources = new[] { jornada };
    }

    public Task<NewsSource?> GetActiveByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        var source = _sources.FirstOrDefault(source =>
            source.State == "A" && string.Equals(source.Alias, alias, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(source);
    }

    private static SourceCategory CreateSourceCategory(long id, NewsSource source, Category category, string name, string url)
    {
        return new SourceCategory
        {
            Id = id,
            NewsSourceId = source.Id,
            CategoryId = category.Id,
            Name = name,
            Url = url,
            State = "A",
            NewsSource = source,
            Category = category
        };
    }
}
