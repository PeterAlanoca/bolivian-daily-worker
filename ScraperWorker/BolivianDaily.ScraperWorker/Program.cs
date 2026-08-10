using BolivianDaily.ScraperWorker.Application.DependencyInjection;
using BolivianDaily.ScraperWorker.Infrastructure.DependencyInjection;
using BolivianDaily.ScraperWorker.Infrastructure.Persistence;
using BolivianDaily.ScraperWorker.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddScraperApplication()
    .AddScraperInfrastructure(builder.Configuration);

builder.Services.AddHostedService<JornadaScrapingWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var scraperDbContext = scope.ServiceProvider.GetRequiredService<ScraperDbContext>();
    scraperDbContext.Database.Migrate();
}

host.Run();
