using System.Globalization;
using System.Xml.Linq;
using ECB.Application;
using ECB.Infrastructure.Application;
using Microsoft.Extensions.Options;

namespace ECB.Infrastructure.Gateway
    {
    /// <summary>
    /// Gateway class for fetching and parsing ECB exchange rates.
    /// </summary>
    public class EcbRatesGateway : IEcbRatesGateway
        {
        private readonly string EcbUrl;
        private readonly HttpClient EcbHttpClient;

        /// <summary>
        /// Constructs the gateway with an injected HttpClient and ECB sync options.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for HTTP requests.</param>
        /// <param name="options">Options containing the ECB URL.</param>
        public EcbRatesGateway(HttpClient httpClient, IOptions<EcbRatesSyncOptions> options)
            {
            EcbHttpClient = httpClient;
            EcbUrl = options.Value.EcbUrl;
            }

        /// <summary>
        /// Fetches the latest exchange rates from the ECB as a list of CurrencyRateDto.
        /// </summary>
        /// <returns>List of currency rates with their value and date.</returns>
        public async Task<List<CurrencyRateDto>> GetExchangeRatesAsync()
            {
            // Using a new HttpClient here; consider using the injected one for reuse & better resource management
            using (var EcbHttpClient = new HttpClient())
                {
                HttpResponseMessage response = await EcbHttpClient.GetAsync(EcbUrl);
                response.EnsureSuccessStatusCode();
                string xmlContent = await response.Content.ReadAsStringAsync();
                return ParseExchangeRates(xmlContent);
                }
            }

        /// <summary>
        /// Parses ECB XML content into a list of CurrencyRateDto.
        /// </summary>
        /// <param name="xmlContent">ECB XML string.</param>
        /// <returns>List of parsed currency rates.</returns>
        public List<CurrencyRateDto> ParseExchangeRates(string xmlContent)
            {
            List<CurrencyRateDto> rates = new List<CurrencyRateDto>();
            XDocument doc = XDocument.Parse(xmlContent);

            XNamespace ecb = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";
            // Find the Cube element with the time attribute (which represents the date)
            XElement? cubeRoot = doc.Descendants(ecb + "Cube")
                                    .FirstOrDefault(c => c.Attribute("time") != null);

            if (cubeRoot != null)
                {
                // Parse the date from the "time" attribute, or use today's date if missing
                DateTime date = DateTime.Parse(cubeRoot.Attribute("time")?.Value ?? DateTime.UtcNow.ToString("yyyy-MM-dd"));
                // Iterate through all currency Cube children
                foreach (XElement cube in cubeRoot.Elements(ecb + "Cube"))
                    {
                    string currency = cube.Attribute("currency")?.Value ?? string.Empty;
                    string rateStr = cube.Attribute("rate")?.Value ?? "0";
                    if (decimal.TryParse(rateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate))
                        {
                        rates.Add(new CurrencyRateDto(currency, rate, date));
                        }
                    }
                }
            return rates;
            }
        }
    }