

namespace ECB.Application
    {
    public interface IEcbRatesGateway
        {
        Task<List<CurrencyRateDto>> GetExchangeRatesAsync();
        List<CurrencyRateDto> ParseExchangeRates(string xmlContent);
        }
    }