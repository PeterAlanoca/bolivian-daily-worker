namespace BolivianDaily.Application.Interfaces;

using BolivianDaily.Domain.Entities;

public interface IExternalNewsApiClient
{
    Task<bool> SubmitNewsAsync(News newsItem, CancellationToken cancellationToken = default);
}
