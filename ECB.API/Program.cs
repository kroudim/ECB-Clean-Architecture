
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using ECB.Application;
using ECB.Infrastructure.Persistence;
using ECB.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using ECB.Infrastructure.Gateway.Service;
using ECB.Infrastructure.Gateway;
using ECB.Infrastructure.Application;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EcbRatesSyncOptions>(
    builder.Configuration.GetSection("EcbRatesSyncOptions"));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    var ecbOptions = builder.Configuration.GetSection("EcbRatesSyncOptions").Get<EcbRatesSyncOptions>();
    options.Configuration = ecbOptions.Redis;
    options.Configuration = builder.Configuration.GetSection("EcbRatesSyncOptions:Redis").Value;
    options.InstanceName = "ECB:";
});
builder.Services.AddSingleton<ICurrencyRatesCache,CurrencyRatesCache>();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseMiddleware<RateLimitingMiddleware>();

// Configure the HTTP request pipeline.file:///C:/Users/DKroustalis/AppData/Local/Postman/app-10.24.26/resources/app.asar/html/scratchpad.html?browserWindowId=1&logPath=C:\Users\DKroustalis\AppData\Roaming\Postman\logs&sessionId=17784&startTime=1751978755899&preloadFile=C:\Users\DKroustalis\AppData\Local\Postman\app-10.24.26\resources\app.asar\preload\desktop\index.js&scratchpadPartitionId=c3416dfd-c071-4d1a-a977-dfa1bd97d22a&isFirstRequester=true
if (app.Environment.IsDevelopment())
  {
  app.UseSwagger();
  app.UseSwaggerUI();
  }

app.UseHttpsRedirection();

app.UseRouting();


app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});

app.Run();
