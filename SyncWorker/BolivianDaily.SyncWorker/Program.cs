using BolivianDaily.SyncWorker.Application.DependencyInjection;
using BolivianDaily.SyncWorker;
using BolivianDaily.SyncWorker.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSyncApplication()
    .AddSyncInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
