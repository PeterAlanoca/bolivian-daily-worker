using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Infrastructure.Configuration;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;

namespace BolivianDaily.CheckerWorker.Infrastructure.AI;

public sealed class OpenRouterArticleChecker(
    HttpClient httpClient,
    IOptions<OpenRouterOptions> openRouterOptions,
    IOptions<CheckerOptions> checkerOptions) : IAiArticleChecker
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ProcessedArticle> CheckAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default)
    {
        var options = openRouterOptions.Value;
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return CreateFallbackArticle(message, "OpenRouter ApiKey is not configured");
        }

        httpClient.BaseAddress ??= new Uri(options.BaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);

        var request = new OpenRouterChatRequest(
            options.Model,
            new[]
            {
                new OpenRouterMessage("system", BuildSystemPrompt()),
                new OpenRouterMessage("user", JsonSerializer.Serialize(message, JsonOptions))
            });

        using var response = await httpClient.PostAsJsonAsync("chat/completions", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var completion = await response.Content.ReadFromJsonAsync<OpenRouterChatResponse>(JsonOptions, cancellationToken);
        var content = completion?.Choices.FirstOrDefault()?.Message.Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            return CreateFallbackArticle(message, "OpenRouter returned an empty response");
        }

        var cleaned = ExtractJson(content);
        var checkedArticle = JsonSerializer.Deserialize<CheckedArticleResponse>(cleaned, JsonOptions);
        if (checkedArticle is null || string.IsNullOrWhiteSpace(checkedArticle.Title) || string.IsNullOrWhiteSpace(checkedArticle.Body))
        {
            return CreateFallbackArticle(message, "OpenRouter response did not include title/body");
        }

        return new ProcessedArticle
        {
            SourceArticleId = message.ArticleId,
            SourceId = message.SourceId,
            CategoryId = checkedArticle.CategoryId ?? message.CategoryId,
            UserId = checkerOptions.Value.UserId,
            Title = checkedArticle.Title,
            Pretitle = checkedArticle.Pretitle,
            Subtitle = checkedArticle.Subtitle,
            Enter = checkedArticle.Enter,
            Body = checkedArticle.Body,
            Author = checkedArticle.Author ?? message.Author,
            PublicationDate = checkedArticle.PublicationDate ?? message.PublicationDate,
            WarningsJson = JsonSerializer.Serialize(checkedArticle.Warnings ?? Array.Empty<string>(), JsonOptions),
            Multimedia = message.Multimedia.Select(media => new ProcessedArticleMedia
            {
                Url = media.Url,
                Type = media.Type,
                Description = media.Description,
                Path = media.Path
            }).ToList()
        };
    }

    private ProcessedArticle CreateFallbackArticle(ArticleScrapedEvent message, string warning)
    {
        return new ProcessedArticle
        {
            SourceArticleId = message.ArticleId,
            SourceId = message.SourceId,
            CategoryId = message.CategoryId,
            UserId = checkerOptions.Value.UserId,
            Title = message.Title,
            Pretitle = message.Pretitle,
            Subtitle = message.Subtitle,
            Enter = message.Lead,
            Body = message.RawBody ?? message.Lead ?? message.Title,
            Author = message.Author,
            PublicationDate = message.PublicationDate,
            WarningsJson = JsonSerializer.Serialize(new[] { warning }, JsonOptions),
            Multimedia = message.Multimedia.Select(media => new ProcessedArticleMedia
            {
                Url = media.Url,
                Type = media.Type,
                Description = media.Description,
                Path = media.Path
            }).ToList()
        };
    }

    private static string BuildSystemPrompt()
    {
        return """
        Eres un editor periodistico. Limpia publicidad y ruido del articulo, confirma la categoria y devuelve solamente JSON valido.
        El JSON debe tener estas propiedades: title, pretitle, subtitle, enter, body, author, publication_date, category_id, warnings.
        body debe estar en HTML limpio. No incluyas markdown ni texto fuera del JSON.
        """;
    }

    private static string ExtractJson(string content)
    {
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');
        return start >= 0 && end > start ? content[start..(end + 1)] : content;
    }

    private sealed record OpenRouterChatRequest(string Model, IReadOnlyCollection<OpenRouterMessage> Messages);
    private sealed record OpenRouterMessage(string Role, string Content);
    private sealed record OpenRouterChatResponse(IReadOnlyCollection<OpenRouterChoice> Choices);
    private sealed record OpenRouterChoice(OpenRouterMessage Message);

    private sealed record CheckedArticleResponse(
        string Title,
        string? Pretitle,
        string? Subtitle,
        string? Enter,
        string Body,
        string? Author,
        DateTime? PublicationDate,
        long? CategoryId,
        IReadOnlyCollection<string>? Warnings);
}
