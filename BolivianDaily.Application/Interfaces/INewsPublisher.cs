using BolivianDaily.Domain.Entities;

namespace BolivianDaily.Application.Interfaces;

public interface INewsPublisher
{
    Task<bool> PublishAsync(News news, CancellationToken cancellationToken = default);
}
