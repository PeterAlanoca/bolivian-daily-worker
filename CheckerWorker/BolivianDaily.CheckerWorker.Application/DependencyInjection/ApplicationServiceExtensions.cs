using BolivianDaily.CheckerWorker.Application.UseCases.ProcessScrapedArticle;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.CheckerWorker.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddCheckerApplication(this IServiceCollection services)
    {
        services.AddScoped<ProcessScrapedArticleUseCase>();
        return services;
    }
}
