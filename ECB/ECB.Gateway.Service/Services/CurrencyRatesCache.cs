using ECB.Application;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ECB.Infrastructure.Gateway.Service
    {
    /// <summary>
    /// Provides a distributed cache implementation for storing and retrieving currency rates.
    /// Implements ICurrencyRatesCache interface.
    /// </summary>
    public class CurrencyRatesCache : ICurrencyRatesCache
        {
        private readonly IDistributedCache _cache;
        private const string CacheKey = "CurrencyRates";

        /// <summary>
        /// Constructs the CurrencyRatesCache with a distributed cache dependency.
        /// </summary>
        /// <param name="distributedCache">The IDistributedCache implementation to use (e.g., Redis).</param>
        public CurrencyRatesCache(IDistributedCache distributedCache)
            {
            _cache = distributedCache;
            }

        /// <summary>
        /// Gets the cached exchange rate for the given currency code.
        /// </summary>
        /// <param name="currency">The currency code to look up.</param>
        /// <returns>The cached exchange rate if found; otherwise, null.</returns>
        public decimal? GetRate(string currency)
            {
            var rates = GetRatesDictionary();
            if (rates != null && rates.TryGetValue(currency, out var rate))
                return rate;
            return null;
            }

        /// <summary>
        /// Tries to retrieve the rates dictionary from the distributed cache.
        /// </summary>
        /// <returns>A dictionary of currency codes and their rates, or null if not found or deserialization fails.</returns>
        private Dictionary<string, decimal> GetRatesDictionary()
            {
            try
                {
                var json = _cache.GetString(CacheKey);
                if (string.IsNullOrEmpty(json))
                    return null;
                return JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
                }
            catch (Exception Ex)
                {
                // Log the exception if desired
                return null;
                }
            }

        /// <summary>
        /// Serializes and stores the provided rates dictionary in the distributed cache.
        /// </summary>
        /// <param name="rates">A dictionary of currency codes and their rates.</param>
        public void SetRates(Dictionary<string, decimal> rates)
            {
            try
                {
                var json = JsonSerializer.Serialize(rates);
                _cache.SetString(CacheKey, json);
                }
            catch (Exception ex)
                {
                // Log the exception if desired
                return;
                }
            }
        }
    }