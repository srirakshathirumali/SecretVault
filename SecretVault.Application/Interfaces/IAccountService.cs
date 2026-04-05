using SecretVault.Application.DTOs.Account;

namespace SecretVault.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request, Guid userId);
        Task<IEnumerable<AccountResponseDto>> GetUserAccountsAsync(Guid userId);
        Task<AccountResponseDto> GetAccountByAccountId(Guid accountId, Guid userId);
        Task<string> GetStatementUrlAsync(Guid accountId, Guid userId);
    }
}
