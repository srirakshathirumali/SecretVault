using Microsoft.EntityFrameworkCore;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;

namespace SecretVault.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task AddAsync(Account account)
        {
            account.Id = Guid.NewGuid();
            account.CreatedOn = DateTime.UtcNow;
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts.Where(a => a.UserId == userId && a.IsActive).ToListAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            var existingAccount = await _context.Accounts.FindAsync(account.Id);
            if (existingAccount == null) return;

            existingAccount.Balance = account.Balance;
            existingAccount.IsActive = account.IsActive;

            _context.Accounts.Update(existingAccount);
            await _context.SaveChangesAsync();

        }
    }
}
