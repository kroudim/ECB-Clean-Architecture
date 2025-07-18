
using ECB.Application;
using ECB.Domain;
using ECB.Persistence;
using Microsoft.EntityFrameworkCore;

    namespace ECB.Infrastructure.Persistence
        {
    public class WalletRepository : IWalletRepository
        {
        private readonly ECBContext _context;

        public WalletRepository(ECBContext context)
            {
            _context = context;
            }

        public async Task<ECB.Domain.Wallet?> GetByIdAsync(long id) =>
            await _context.Wallets.FindAsync(id);

        public async Task CreateAsync(Wallet wallet)
            {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            }

        public async Task UpdateAsync(Wallet wallet)
            {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            }
        }      
        }
    