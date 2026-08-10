using System.Net.Http.Json;
using System.Text.Json;
using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Infrastructure.Configuration;
using BolivianDaily.CheckerWorker.Infrastructure.Mappers;
using BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter;

public sealed class OpenRouterArticleChecker(
    HttpClient httpClient,
    IOptions<OpenRouterOptions> options) : IArticleChecker
{
    public async Task<CheckedArticle> CheckAsync(ArticleScrapedEvent articleScrapedEvent, CancellationToken cancellationToken = default)
    {
        var openRouterChatRequest = articleScrapedEvent.ToOpenRouterChatRequest(
            model: options.Value.Model,
            prompt: options.Value.Prompt);

        using var response = await httpClient.PostAsJsonAsync("chat/completions", openRouterChatRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var openRouterChatResponse = await response.Content.ReadFromJsonAsync<OpenRouterChatResponse>(cancellationToken);
        var content = openRouterChatResponse?.Choices.FirstOrDefault()?.Message.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            return articleScrapedEvent.ToCheckedArticle("OpenRouter returned an empty response");
        }

        var articleValidationResponse = JsonSerializer.Deserialize<ArticleValidationResponse>(content);
        if (articleValidationResponse is null)
        {
            return articleScrapedEvent.ToCheckedArticle("OpenRouter returned an unparseable response");
        }

        if (!articleValidationResponse.IsValid)
        {
            var fallback = articleValidationResponse.ToCheckedArticle(articleScrapedEvent);
            fallback.Warnings ??= "Article failed AI validation";
            return fallback;
        }

        return articleValidationResponse.ToCheckedArticle(articleScrapedEvent);
    }

}
