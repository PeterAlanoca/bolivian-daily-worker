using BolivianDaily.Application.Dtos;
using BolivianDaily.Domain.Entities;

namespace BolivianDaily.Application.Interfaces;

public interface INewsContentAnalyst
{
    Task<NewsAnalysisResult> AnalyzeAsync(News news, string categoryName, CancellationToken cancellationToken = default);
}
