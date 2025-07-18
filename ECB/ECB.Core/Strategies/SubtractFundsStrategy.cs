using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain
  {
  public class SubtractFundsStrategy : IBalanceStrategy
    {
    public void Adjust(Wallet wallet, decimal amount)
      {
      if (wallet.Balance < amount)
        throw new InvalidOperationException("Insufficient funds");
      wallet.Balance -= amount;
      }
    }
  }
