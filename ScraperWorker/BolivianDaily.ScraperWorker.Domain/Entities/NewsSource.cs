namespace BolivianDaily.ScraperWorker.Domain.Entities;

public class NewsSource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string State { get; set; } = "A";
    public List<SourceCategory> Categories { get; set; } = new();
}
