using Microsoft.EntityFrameworkCore;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;

namespace SecretVault.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid();
            transaction.Timestamp = DateTime.UtcNow;
            await _context.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Transaction> transactions)
        {
            foreach (var item in transactions)
            {
                item.Id = Guid.NewGuid();
                item.Timestamp = DateTime.UtcNow;
            }
            await _context.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Transactions.Where(t => t.AccountId == accountId).OrderByDescending(t => t.Timestamp).ToListAsync();
        }
    }
}
