using ECB.Application;
using ECB.Infrastructure.Application;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ECB.Infrastructure.Gateway.Service
    {
    /// <summary>
    /// Provides methods for managing database connections and transactions for ECB rates synchronization.
    /// </summary>
    public class DatabaseService : IDatabaseService
        {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        /// <summary>
        /// Initializes the DatabaseService with the connection string from options.
        /// </summary>
        /// <param name="options">Options containing the ECB rates connection string.</param>
        public DatabaseService(IOptions<EcbRatesSyncOptions> options)
            {
            _connectionString = options.Value.ECBRatesConnectionString;
            }

        /// <summary>
        /// Opens a new database connection asynchronously.
        /// </summary>
        public async Task OpenConnectionAsync()
            {
            _connection = new SqlConnection(_connectionString);
            await _connection.OpenAsync();
            }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        public Task BeginTransactionAsync()
            {
            _transaction = _connection.BeginTransaction();
            return Task.CompletedTask;
            }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public Task CommitTransactionAsync()
            {
            _transaction.Commit();
            return Task.CompletedTask;
            }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public Task RollbackTransactionAsync()
            {
            _transaction.Rollback();
            return Task.CompletedTask;
            }

        /// <summary>
        /// Executes a non-query SQL command asynchronously using the current connection and transaction.
        /// </summary>
        /// <param name="commandText">The SQL command to execute.</param>
        /// <param name="parameters">Parameters for the SQL command.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText, params SqlParameter[] parameters)
            {
            using (var command = new SqlCommand(commandText, _connection, _transaction))
                {
                command.Parameters.AddRange(parameters);
                return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }