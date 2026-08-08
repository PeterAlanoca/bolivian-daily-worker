using BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.SyncWorker.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddSyncApplication(this IServiceCollection services)
    {
        services.AddScoped<SyncCheckedArticleUseCase>();
        return services;
    }
}
