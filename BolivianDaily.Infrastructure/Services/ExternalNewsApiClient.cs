namespace BolivianDaily.Infrastructure.Services;

using System.Text;
using System.Text.Json;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;

public class ExternalNewsApiClient : IExternalNewsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalNewsApiClient> _logger;
    private readonly string _apiUrl = "https://example.com/api/v1/news";

    public ExternalNewsApiClient(HttpClient httpClient, ILogger<ExternalNewsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SubmitNewsAsync(News newsItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var jsonContent = JsonSerializer.Serialize(newsItem, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Submitting News '{newsItem.Title}' to External API...");
            var response = await _httpClient.PostAsync(_apiUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully submitted news to External API.");
                return true;
            }

            _logger.LogWarning($"Failed to submit news. Status Code: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while submitting news to External API.");
            return false;
        }
    }
}
