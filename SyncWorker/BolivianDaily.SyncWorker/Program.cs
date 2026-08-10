using BolivianDaily.SyncWorker.Application.DependencyInjection;
using BolivianDaily.SyncWorker;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using BolivianDaily.SyncWorker.Infrastructure.DependencyInjection;
using BolivianDaily.SyncWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSyncApplication()
    .AddSyncInfrastructure(builder.Configuration);

var extranetOptions = builder.Configuration
    .GetSection(ExtranetOptions.SectionName)
    .Get<ExtranetOptions>();
if (extranetOptions is null || string.IsNullOrWhiteSpace(extranetOptions.Url))
{
    throw new InvalidOperationException("Extranet:Url is not configured");
}

builder.Services.AddHostedService<SyncConsumer>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var syncDbContext = scope.ServiceProvider.GetRequiredService<SyncDbContext>();
    syncDbContext.Database.Migrate();
}

host.Run();
