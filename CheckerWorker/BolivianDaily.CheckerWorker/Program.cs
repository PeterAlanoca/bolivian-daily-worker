using BolivianDaily.CheckerWorker.Application.DependencyInjection;
using BolivianDaily.CheckerWorker;
using BolivianDaily.CheckerWorker.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddCheckerApplication()
    .AddCheckerInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
