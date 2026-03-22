namespace BolivianDaily.Application.UseCases.Scraping;

using MediatR;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

public class ScrapeSourcesCommandHandler : IRequestHandler<ScrapeSourcesCommand, bool>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly INewsRepository _newsRepository;
    private readonly IScraperFactory _scraperFactory;
    private readonly INewsPublisher _newsPublisher;
    private readonly INewsContentAnalyst _newsAnalyst;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ScrapeSourcesCommandHandler> _logger;

    public ScrapeSourcesCommandHandler(
        ISourceRepository sourceRepository,
        INewsRepository newsRepository,
        IScraperFactory scraperFactory,
        INewsPublisher newsPublisher,
        INewsContentAnalyst newsAnalyst,
        IConfiguration configuration,
        ILogger<ScrapeSourcesCommandHandler> logger)
    {
        _sourceRepository = sourceRepository;
        _newsRepository = newsRepository;
        _scraperFactory = scraperFactory;
        _newsPublisher = newsPublisher;
        _newsAnalyst = newsAnalyst;
        _configuration = configuration;
        _logger = logger;
    }

    private void LogRich(string message, string color = "white")
    {
        var timestamp = $"[silver][[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]][/]";
        AnsiConsole.MarkupLine($"{timestamp} [{color}]{Markup.Escape(message)}[/]");
    }

    public async Task<bool> Handle(ScrapeSourcesCommand request, CancellationToken cancellationToken)
    {
        var submitNewsDelay = _configuration.GetValue<int>("ScrapingOptions:DelayBetweenArticlesMs", 1000);

        LogRich("INICIANDO PROCESO", "cyan");

        var activeSources = await _sourceRepository.GetAllActiveAsync(cancellationToken);
        LogRich($"Se encontraron {activeSources.Count()} fuentes activas para procesar.", "yellow");

        foreach (var sourceItem in activeSources)
        {
            if (!_scraperFactory.HasScraperFor(sourceItem.Alias ?? string.Empty))
            {
                LogRich($"SALTANDO: No hay un scraper registrado para la fuente '{sourceItem.Name}' (alias: '{sourceItem.Alias}')", "red");
                continue;
            }

            var scraper = _scraperFactory.GetFor(sourceItem.Alias!);
            LogRich($"PROCESANDO FUENTE: {sourceItem.Name} (ID: {sourceItem.Id})", "bold yellow");

            foreach (var sourceCategory in sourceItem.Categories)
            {
                var categoryName = sourceCategory.Category?.Name ?? "General";
                LogRich($"CATEGORÍA: {categoryName}", "blue");

                var articleUrls = await scraper.GetLatestArticleUrlsAsync(sourceCategory, cancellationToken);
                LogRich($"Se encontraron {articleUrls.Count} URLs de artículos para analizar.", "silver");

                foreach (var url in articleUrls)
                {
                    LogRich($"ANALIZANDO URL: {url}", "gray");

                    var exists = await _newsRepository.ExistsByUrlAsync(url, cancellationToken);
                    if (exists)
                    {
                        LogRich("Resultado: SALTADO (Ya existe en la base de datos)", "silver");
                        continue;
                    }

                    try
                    {
                        var newsArticle = await scraper.ScrapeArticleAsync(url, cancellationToken);
                        if (newsArticle != null)
                        {
                            LogRich("Acción: Realizando Análisis de Contenido con IA...", "purple");
                            var analystResult = await _newsAnalyst.AnalyzeAsync(newsArticle, categoryName, cancellationToken);

                            var aiStatus = analystResult.IsEnabled ? "[IA: HABILITADA]" : "[IA: DESHABILITADA - PASE POR DEFECTO]";

                            if (!analystResult.IsValid)
                            {
                                LogRich($"Resultado: RECHAZADO {aiStatus} - Confianza: {analystResult.Confidence}", "red");
                                LogRich($"Problemas: {string.Join(", ", analystResult.Issues)}", "red");
                                continue;
                            }

                            var resultColor = analystResult.IsEnabled ? "green" : "yellow";
                            LogRich($"Resultado: APROBADO {aiStatus} - Confianza: {analystResult.Confidence}", resultColor);

                            newsArticle.CategoryId = sourceCategory.CategoryId;
                            newsArticle.SourceId = sourceItem.Id;

                            await _newsRepository.AddAsync(newsArticle, cancellationToken);
                            LogRich("Base de datos: Guardado exitosamente en el repositorio.", "green");

                            await _newsPublisher.PublishAsync(newsArticle, cancellationToken);
                            LogRich("Publicador: Enviado exitosamente al API de BolivianDaily.", "green");

                            LogRich($"ÉXITO: Procesado artículo '{newsArticle.Title}'", "bold green");

                            LogRich($"Espera: Esperando {submitNewsDelay}ms antes del siguiente artículo...", "gray");
                            await Task.Delay(submitNewsDelay, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogRich($"EXCEPCIÓN: {ex.Message}", "bold red");
                        _logger.LogError(ex, "Error al procesar el artículo en {Url}", url);
                    }
                }
            }
        }

        LogRich("PROCESO DE RASPADO FINALIZADO EXITOSAMENTE", "bold cyan");
        return true;
    }
}
