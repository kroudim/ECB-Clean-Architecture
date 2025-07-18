using ECB.Infrastructure.Application;
using ECB.Infrastructure.Gateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Register the strongly-typed options
        services.Configure<EcbRatesSyncOptions>(context.Configuration.GetSection("EcbRatesSyncOptions"));
        // Register the gateway with HttpClient and options support
        services.AddHttpClient<EcbRatesGateway>();
    })
    .Build();

var ecbService = host.Services.GetRequiredService<EcbRatesGateway>();
var rates = await ecbService.GetExchangeRatesAsync();

foreach (var rate in rates)
    {
    Console.WriteLine($"Currency: {rate.Currency}, Rate: {rate.Rate}, Date: {rate.Date}");
    }