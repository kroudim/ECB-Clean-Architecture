
using ECB.Application;
using ECB.Domain;
using ECB.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECB.Infrastructure.Persistence
    {
    public class CurrencyRatesRepository : ICurrencyRatesRepository
        {
        private readonly ECBContext _context;

        public CurrencyRatesRepository(ECBContext context)
            {
            _context = context;
            }

        /// <summary>
        /// Get the lastest currency rate for the specified currency.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<decimal> GetCurrencyRateAsync(string currency)
            {
            var cur_rate = await _context.CurrencyRates.OrderByDescending(x=>x.Date).LastOrDefaultAsync(x=>x.Currency == currency);
            return cur_rate?.Rate ?? throw new KeyNotFoundException($"Currency rate for {currency} not found.");
            }
        }
    }
    