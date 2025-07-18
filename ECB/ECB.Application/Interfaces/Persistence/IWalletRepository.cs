using ECB.Domain;

namespace ECB.Application
    {
    public interface IWalletRepository
        {
        Task<Wallet?> GetByIdAsync(long id);
        Task CreateAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        }
    }