using SecretVault.Application.DTOs.Transaction;

namespace SecretVault.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> DepositAsync(DepositRequestDto request, Guid userId);

        Task<TransactionResponseDto> WithdrawAsync(WithdrawRequestDto request, Guid userId);
        Task<TransactionResponseDto> TransferAsync(TransferRequestDto request, Guid userId);
        Task<IEnumerable<TransactionResponseDto>> GetHistoryAsync(Guid accountId, Guid userId);
    }
}
