using BolivianDaily.ScraperWorker.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace BolivianDaily.ScraperWorker.Infrastructure.Configuration;

public class JornadaOptionsProvider(IOptions<JornadaOptions> options) : IJornadaOptionsProvider
{
    public string Alias => options.Value.Alias;
    public int IntervalMinutes => options.Value.IntervalMinutes;
    public int CategoryDelayMs => options.Value.CategoryDelayMs;
    public int MinArticleDelayMs => options.Value.MinArticleDelayMs;
    public int MaxArticleDelayMs => options.Value.MaxArticleDelayMs;
}

