using ECB.Application;
using ECB.Infrastructure.Gateway;
using ECB.Infrastructure.Gateway.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ECB.Infrastructure.Application;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration (appsettings.json is added by default in .NET 8+)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Bind configuration for options
builder.Services.Configure<EcbRatesSyncOptions>(builder.Configuration.GetSection("EcbRatesSyncOptions"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("EcbRatesSyncOptions:Redis").Value;
    options.InstanceName = "ECB:";
});
builder.Services.AddSingleton<ICurrencyRatesCache, CurrencyRatesCache>();
builder.Services.AddHttpClient<EcbRatesGateway>();
builder.Services.AddHttpClient<IEcbRatesGateway, EcbRatesGateway>();

// Register DatabaseService using options pattern for connection string
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

// Register the hosted service (constructor will get everything via DI)
builder.Services.AddHostedService<EcbRatesSyncService>();

var host = builder.Build();
host.Run();