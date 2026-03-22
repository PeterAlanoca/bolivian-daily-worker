using System.Text.Json;
using System.Text.RegularExpressions;
using BolivianDaily.Application.Dtos;
using BolivianDaily.Domain.Entities;
using BolivianDaily.Infrastructure.ExternalServices.DeepSeek.Dtos;

namespace BolivianDaily.Infrastructure.ExternalServices.DeepSeek.Mappers;

public static class DeepSeekMapper
{
    public static DeepSeekChatRequest ToDeepSeekRequest(News news, string categoryName, string model, string systemPrompt, string userPromptTemplate)
    {
        var userPrompt = string.Format(userPromptTemplate, 
            categoryName, 
            news.Title, 
            news.Body?.Replace("\"", "\\\""));

        return new DeepSeekChatRequest
        {
            Model = model,
            Messages = new List<DeepSeekChatMessageDto>
            {
                new DeepSeekChatMessageDto { Role = "system", Content = systemPrompt },
                new DeepSeekChatMessageDto { Role = "user", Content = userPrompt }
            }
        };
    }

    public static NewsAnalysisResult ToAnalysisResult(string aiContent)
    {
        if (string.IsNullOrEmpty(aiContent))
        {
            return new NewsAnalysisResult { IsValid = false, Issues = new List<string> { "AI returned empty content" } };
        }

        try
        {
            // Extract JSON from markdown code block if present
            var jsonMatch = Regex.Match(aiContent, @"```json\s*(\{.*?\})\s*```", RegexOptions.Singleline);
            var jsonString = jsonMatch.Success ? jsonMatch.Groups[1].Value : aiContent;

            var result = JsonSerializer.Deserialize<NewsAnalysisResult>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new NewsAnalysisResult { IsValid = false, Issues = new List<string> { "Failed to deserialize AI response" } };
            
            result.IsEnabled = true;
            return result;
        }
        catch (Exception ex)
        {
            return new NewsAnalysisResult { IsValid = false, Issues = new List<string> { $"Error parsing AI response: {ex.Message}" } };
        }
    }
}
