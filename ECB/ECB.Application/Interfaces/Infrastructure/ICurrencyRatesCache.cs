
namespace ECB.Application
    {
    public interface ICurrencyRatesCache
        {
        decimal? GetRate(string currency);
        void SetRates(Dictionary<string, decimal> rates);
        }
    }