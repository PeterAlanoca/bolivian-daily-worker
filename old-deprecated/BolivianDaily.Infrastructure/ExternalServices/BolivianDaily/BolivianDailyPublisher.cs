using System.Text;
using System.Text.Json;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Infrastructure.ExternalServices.BolivianDaily.Dtos;
using BolivianDaily.Infrastructure.ExternalServices.BolivianDaily.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.Infrastructure.ExternalServices.BolivianDaily;

public class BolivianDailyPublisher : INewsPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BolivianDailyPublisher> _logger;
    private readonly string _apiUrl;

    public BolivianDailyPublisher(HttpClient httpClient, IConfiguration configuration, ILogger<BolivianDailyPublisher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiUrl = configuration["ExternalApi:BaseUrl"] ?? string.Empty;
        
        var token = configuration["ExternalApi:Token"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-TOKEN", token);
        }
    }

    public async Task<bool> PublishAsync(News news, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = news.ToBolivianDailyDto();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var jsonContent = JsonSerializer.Serialize(dto, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Publicando noticia '{Title}' en el API de BolivianDaily...", news.Title);
            var response = await _httpClient.PostAsync(_apiUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                _logger.LogInformation("Noticia publicada exitosamente en BolivianDaily.");
                return true;
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Fallo al publicar la noticia. Estado: {StatusCode}, Error: {Error}", response.StatusCode, errorBody);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al publicar la noticia en BolivianDaily.");
            return false;
        }
    }
}
