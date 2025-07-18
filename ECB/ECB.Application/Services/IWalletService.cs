
using ECB.Domain;
using System.Threading.Tasks;

namespace ECB.Application
  {
  public interface IWalletService
    {
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task<decimal> GetBalanceAsync(long walletId, string currency);
    Task AdjustBalanceAsync(long walletId, decimal amount, string currency, IBalanceStrategy strategy);
    }
  }
