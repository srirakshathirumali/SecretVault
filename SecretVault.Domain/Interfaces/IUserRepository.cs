using SecretVault.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAync(User user);
        Task UpdateAync(User user);
    }
}
