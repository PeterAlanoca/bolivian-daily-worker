using BolivianDaily.CheckerWorker.Application.UseCases.CheckScrapedArticle;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.CheckerWorker.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddCheckerApplication(this IServiceCollection services)
    {
        services.AddScoped<CheckScrapedArticleUseCase>();
        return services;
    }
}
