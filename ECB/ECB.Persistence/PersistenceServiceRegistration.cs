using ECB.Application;
using ECB.Infrastructure.Application;
using ECB.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace ECB.Infrastructure.Persistence
    {
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ECBContext>((provider, dbContextOptions) =>
            {
                var options = provider.GetRequiredService<IOptions<EcbRatesSyncOptions>>().Value;
                dbContextOptions.UseSqlServer(options.ECBRatesConnectionString);
            });

          services.AddScoped<IWalletRepository,WalletRepository >();
          services.AddScoped<ICurrencyRatesRepository, CurrencyRatesRepository>();
            return services;    
        }
    }
}
