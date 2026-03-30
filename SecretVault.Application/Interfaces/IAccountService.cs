using SecretVault.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request, Guid userId);
        Task<IEnumerable<AccountResponseDto>> GetUserAccountsAsync(Guid userId);
        Task<AccountResponseDto> GetAccountByAccountId(Guid accountId, Guid userId);
    }
}
