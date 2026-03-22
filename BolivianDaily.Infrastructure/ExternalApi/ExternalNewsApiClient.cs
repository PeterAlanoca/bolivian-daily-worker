namespace BolivianDaily.Infrastructure.ExternalApi;

using System.Text;
using System.Text.Json;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using Microsoft.Extensions.Logging;
using BolivianDaily.Infrastructure.ExternalApi.Mappers;

public class ExternalNewsApiClient : IExternalNewsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalNewsApiClient> _logger;
    private readonly string _apiUrl;

    public ExternalNewsApiClient(HttpClient httpClient, ILogger<ExternalNewsApiClient> logger, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _apiUrl = configuration["ExternalApi:BaseUrl"] ?? "http://localhost/api/news";
        var apiToken = configuration["ExternalApi:Token"] ?? string.Empty;

        // Setup headers
        _httpClient.DefaultRequestHeaders.Add("X-API-TOKEN", apiToken);
    }

    public async Task<bool> SubmitNewsAsync(News newsItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = newsItem.ToSubmissionDto();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var jsonContent = JsonSerializer.Serialize(dto, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Submitting News '{Title}' to External API at {Url}...", newsItem.Title, _apiUrl);
            var response = await _httpClient.PostAsync(_apiUrl, content, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Successfully submitted news to External API. Response: {Response}", responseBody);
                return true;
            }

            _logger.LogWarning("Failed to submit news. Status Code: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while submitting news to External API.");
            return false;
        }
    }
}
