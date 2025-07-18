using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain
  {
  public class AddFundsStrategy : IBalanceStrategy
    {
    public void Adjust(Wallet wallet, decimal amount)
      {
      wallet.Balance += amount;
      }
    }
  }


