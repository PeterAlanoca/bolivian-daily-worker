using BolivianDaily.Application.Extensions;
using BolivianDaily.Infrastructure.Extensions;
using BolivianDaily.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Inject Layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Hosted Service (The periodic Web Scraper)
builder.Services.AddHostedService<ScraperWorker>();

var host = builder.Build();
host.Run();
