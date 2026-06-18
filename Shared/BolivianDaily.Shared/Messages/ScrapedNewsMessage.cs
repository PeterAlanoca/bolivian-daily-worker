using System;

namespace BolivianDaily.Shared.Messages;

/// <summary>
/// Representa el mensaje con los datos crudos extraídos por el Scraper
/// que se publicará en RabbitMQ para que el IAWorker lo procese.
/// </summary>
public class ScrapedNewsMessage
{
    public string SourceId { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string RawContent { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime ScrapedAt { get; set; }
}
