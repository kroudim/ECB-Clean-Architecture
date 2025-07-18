using ECB.Domain;
using Microsoft.EntityFrameworkCore;

namespace ECB.Persistence
    {
    public class ECBContext : DbContext
        {
        public DbSet<CurrencyRate> CurrencyRates { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;

        public ECBContext(DbContextOptions<ECBContext> options)
            : base(options)
            {
            }

        // Optionally keep for design-time, but not needed with factory:
        public ECBContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            // Use a fixed date for seeding, not DateTime.Now
            var seedDate = new DateTime(2025, 7, 7);

            modelBuilder.Entity<CurrencyRate>()
                .HasData(
                    new CurrencyRate("USD", 1.0360m, seedDate) { Id = 1 },
                    new CurrencyRate("JPY", 157.95m, seedDate) { Id = 2 },
                    new CurrencyRate("PLN", 4.2065m, seedDate) { Id = 3 }
                );

            modelBuilder.Entity<Wallet>()
                .HasData(
                    new Wallet(1, "USD", 1m),
                    new Wallet(2,"JPY", 2m),
                    new Wallet( 3,"PLN", 3m)
                );

            base.OnModelCreating(modelBuilder);
            }
        }
    }