using BolivianDaily.ScraperWorker.Application.DependencyInjection;
using BolivianDaily.ScraperWorker.Infrastructure.DependencyInjection;
using BolivianDaily.ScraperWorker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddScraperApplication()
    .AddScraperInfrastructure(builder.Configuration);

builder.Services.AddHostedService<JornadaScrapingWorker>();

var host = builder.Build();
host.Run();
