using ECB.Application;
using ECB.Infrastructure.Application;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ECB.Infrastructure.Gateway.Service
    {
    /// <summary>
    /// Background service responsible for periodically synchronizing ECB currency rates with the database and cache.
    /// </summary>
    public class EcbRatesSyncService : IHostedService, IDisposable
        {
        private readonly ILogger<EcbRatesSyncService> _logger;
        private readonly IEcbRatesGateway _ecbService;
        private readonly IDatabaseService _databaseService;
        private readonly ICurrencyRatesCache _cache;
        private readonly double _interval;

        /// <summary>
        /// Constructs the sync service with required dependencies.
        /// </summary>
        /// <param name="logger">Logger for logging information and errors.</param>
        /// <param name="ecbService">Gateway to fetch ECB rates.</param>
        /// <param name="databaseService">Service for database operations.</param>
        /// <param name="cache">Currency rates cache.</param>
        /// <param name="options">Options containing sync interval.</param>
        public EcbRatesSyncService(
            ILogger<EcbRatesSyncService> logger,
            IEcbRatesGateway ecbService,
            IDatabaseService databaseService,
            ICurrencyRatesCache cache,
            IOptions<EcbRatesSyncOptions> options)
            {
            _logger = logger;
            _ecbService = ecbService;
            _databaseService = databaseService;
            _cache = cache;
            _interval = options.Value.ECBRatesInterval;
            }

        /// <summary>
        /// Starts the background sync loop, running until cancellation is requested.
        /// </summary>
        public async Task StartAsync(CancellationToken stoppingToken)
            {
            while (!stoppingToken.IsCancellationRequested)
                {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await DoWorkAsync();
                await Task.Delay(TimeSpan.FromMinutes(_interval), stoppingToken);
                }
            }

        /// <summary>
        /// Fetches latest rates and updates database and cache.
        /// </summary>
        public async Task DoWorkAsync()
            {
            var rates = await _ecbService.GetExchangeRatesAsync();
            await UpdateDatabaseAsync(rates);
            }

        /// <summary>
        /// Updates the currency rates in the database using a MERGE statement, and refreshes the cache.
        /// </summary>
        /// <param name="rates">List of currency rates to update.</param>
        public async Task UpdateDatabaseAsync(List<CurrencyRateDto> rates)
            {
            await _databaseService.OpenConnectionAsync();
            await _databaseService.BeginTransactionAsync();
            try
                {
                foreach (var rate in rates)
                    {
                    // MERGE ensures upsert: updates if exists, inserts if not
                    string mergeQuery = @"MERGE INTO CurrencyRates AS target
                                             USING (SELECT @Currency AS Currency, @Rate AS Rate, @Date AS Date) AS source
                                             ON target.Currency = source.Currency AND target.Date = source.Date
                                             WHEN MATCHED THEN
                                                 UPDATE SET target.Rate = source.Rate
                                             WHEN NOT MATCHED THEN
                                                 INSERT (Currency, Rate, Date) VALUES (source.Currency, source.Rate, source.Date);";
                    var parameters = new[]
                    {
                        new SqlParameter("@Currency", rate.Currency),
                        new SqlParameter("@Rate", rate.Rate),
                        new SqlParameter("@Date", rate.Date)
                    };
                    await _databaseService.ExecuteNonQueryAsync(mergeQuery, parameters);
                    }
                await _databaseService.CommitTransactionAsync();

                // Update the cache with latest rates
                _cache.SetRates(rates.ToDictionary(
                                    rate => rate.Currency,
                                    rate => rate.Rate));
                }
            catch
                {
                // Rollback changes in case of error
                await _databaseService.RollbackTransactionAsync();
                throw;
                }
            }

        /// <summary>
        /// Called when the service is stopping.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
            {
            _logger.LogInformation("ECBJob is stopping.");
            // No timer to dispose as loop is controlled by cancellation token
            return Task.CompletedTask;
            }

        /// <summary>
        /// Disposes resources, if necessary.
        /// </summary>
        public void Dispose()
            {
            // Dispose managed resources here if needed
            }
        }
    }