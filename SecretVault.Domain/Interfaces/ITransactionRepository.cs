using System;
using System.Collections.Generic;
using System.Text;
using Transaction=SecretVault.Domain.Entities.Transaction;

namespace SecretVault.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(Transaction transaction);
        Task AddRangeAsync(IEnumerable<Transaction> transactions);

    }
}
