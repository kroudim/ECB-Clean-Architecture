using ECB.Domain;

namespace ECB.Application
    {
    public interface ICurrencyRatesRepository
        {
        Task<decimal> GetCurrencyRateAsync(string currency);

        }
    }