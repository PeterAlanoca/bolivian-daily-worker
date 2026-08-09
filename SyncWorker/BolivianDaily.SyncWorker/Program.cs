using BolivianDaily.SyncWorker.Application.DependencyInjection;
using BolivianDaily.SyncWorker;
using BolivianDaily.SyncWorker.Infrastructure.DependencyInjection;
using BolivianDaily.SyncWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddSyncApplication()
    .AddSyncInfrastructure(builder.Configuration);

builder.Services.AddHostedService<SyncConsumer>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SyncDbContext>();
    db.Database.Migrate();
}

host.Run();
