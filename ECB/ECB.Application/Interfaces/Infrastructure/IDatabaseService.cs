using Microsoft.Data.SqlClient;

namespace ECB.Application
    {
  public interface IDatabaseService
    {
    Task OpenConnectionAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<int> ExecuteNonQueryAsync(string commandText, params SqlParameter[] parameters);
    }
  }