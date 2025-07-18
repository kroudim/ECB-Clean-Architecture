using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain

  {
  public class ForceSubtractFundsStrategy : IBalanceStrategy
    {
    public void Adjust(Wallet wallet, decimal amount)
      {
      wallet.Balance -= amount;
      }
    }
  }
