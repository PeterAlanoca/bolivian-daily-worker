using System.Net.Http.Json;
using BolivianDaily.Application.Dtos;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Infrastructure.ExternalServices.DeepSeek.Dtos;
using BolivianDaily.Infrastructure.ExternalServices.DeepSeek.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.Infrastructure.ExternalServices.DeepSeek;

public class DeepSeekContentAnalyst : INewsContentAnalyst
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeepSeekContentAnalyst> _logger;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _baseUrl;
    private readonly bool _enabled;
    private readonly string _systemPrompt;
    private readonly string _userPromptTemplate;

    public DeepSeekContentAnalyst(HttpClient httpClient, IConfiguration configuration, ILogger<DeepSeekContentAnalyst> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        var section = configuration.GetSection("DeepSeekApi");
        _enabled = section.GetValue<bool>("Enabled", false);
        _baseUrl = section["BaseUrl"] ?? "https://api.deepseek.com/v1/chat/completions";
        _apiKey = section["ApiKey"] ?? string.Empty;
        _model = section["Model"] ?? "deepseek-chat";
        _systemPrompt = section["SystemPrompt"] ?? string.Empty;
        _userPromptTemplate = section["UserPromptTemplate"] ?? string.Empty;

        if (string.IsNullOrEmpty(_apiKey) && _enabled)
        {
            _logger.LogWarning("DeepSeek API Key is missing in configuration.");
        }

        _logger.LogInformation("DeepSeek Content Analyst initialized. Status: {Status}", _enabled ? "ENABLED" : "DISABLED (USING DEFAULT PASS)");
    }

    public async Task<NewsAnalysisResult> AnalyzeAsync(News news, string categoryName, CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogInformation("DeepSeek analysis is disabled. Skipping validation for article: {Title}", news.Title);
            return new NewsAnalysisResult 
            { 
                IsValid = true, 
                Confidence = 1.0, 
                IsEnabled = false,
                CategoryCorrect = true, 
                HasHtml = true,
                Issues = new List<string>() 
            };
        }

        try
        {
            var request = DeepSeekMapper.ToDeepSeekRequest(news, categoryName, _model, _systemPrompt, _userPromptTemplate);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl)
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

            _logger.LogInformation("Requesting AI content analysis for article: {Title}", news.Title);
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var deepSeekResponse = await response.Content.ReadFromJsonAsync<DeepSeekChatResponse>(cancellationToken: cancellationToken);
            var content = deepSeekResponse?.Choices.FirstOrDefault()?.Message.Content;

            return DeepSeekMapper.ToAnalysisResult(content ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing news content with DeepSeek AI.");
            return new NewsAnalysisResult 
            { 
                IsValid = false, 
                Issues = new List<string> { $"Exception during AI analysis: {ex.Message}" } 
            };
        }
    }
}
