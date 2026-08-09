using BolivianDaily.CheckerWorker.Application.DependencyInjection;
using BolivianDaily.CheckerWorker;
using BolivianDaily.CheckerWorker.Infrastructure.DependencyInjection;
using BolivianDaily.CheckerWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddCheckerApplication()
    .AddCheckerInfrastructure(builder.Configuration);

builder.Services.AddHostedService<CheckerConsumer>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CheckerDbContext>();
    db.Database.Migrate();
}

host.Run();
