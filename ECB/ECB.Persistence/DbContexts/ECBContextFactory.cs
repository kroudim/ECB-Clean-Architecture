using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ECB.Persistence
    {
    public class ECBContextFactory : IDesignTimeDbContextFactory<ECBContext>
        {
        public ECBContext CreateDbContext(string[] args)
            {
            var parentDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var configDir = Path.Combine(parentDir, "ECB.API");

            // Build config (looks for appsettings.json in current directory or parent)
            var config = new ConfigurationBuilder()
                .SetBasePath(configDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Get the connection string from config
            var connectionString = config.GetSection("EcbRatesSyncOptions")["ECBRatesConnectionString"];

            var optionsBuilder = new DbContextOptionsBuilder<ECBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ECBContext(optionsBuilder.Options);
            }
        }
    }