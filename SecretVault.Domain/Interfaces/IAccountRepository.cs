using SecretVault.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task<bool> AccountNumberExistsAsync(string accountNumber);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
    }
}
