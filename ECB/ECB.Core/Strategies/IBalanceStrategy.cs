using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECB.Domain
  {
  public interface IBalanceStrategy
    {    void Adjust(Wallet wallet, decimal amount);
    }
  }
