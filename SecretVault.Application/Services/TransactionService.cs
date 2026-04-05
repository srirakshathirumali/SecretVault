using SecretVault.Application.DTOs.Transaction;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Enums;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;

namespace SecretVault.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }
        public async Task<TransactionResponseDto> DepositAsync(DepositRequestDto request, Guid userId)
        {
            var account = await GetOwnedAccountAsync(request.AccountId, userId);
            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Type = TransactionType.Deposit,
                Amount = request.Amount,
                BalanceAfter = account.Balance,
                Description = request.Description
            };

            await _transactionRepository.AddAsync(transaction);

            await _accountRepository.UpdateAsync(account);

            return MapToTransactionDto(transaction); ;
        }
        public async Task<TransactionResponseDto> WithdrawAsync(WithdrawRequestDto request, Guid userId)
        {
            var account = await GetOwnedAccountAsync(request.AccountId, userId);

            if (account.Balance < request.Amount)
            {
                throw new InsufficientFundsException();
            }

            account.Balance -= request.Amount;
            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Type = TransactionType.Withdraw,
                Amount = request.Amount,
                BalanceAfter = account.Balance,
                Description = request.Description
            };

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            return MapToTransactionDto(transaction);

        }
        public async Task<TransactionResponseDto> TransferAsync(TransferRequestDto request, Guid userId)
        {
            var sourceAccount = await GetOwnedAccountAsync(request.FromAccountId, userId);

            if (sourceAccount.Balance < request.Amount)
            {
                throw new InsufficientFundsException();
            }

            if (sourceAccount.Id == request.ToAccountId)
            {
                throw new SelfTransferException();
            }
            var destinationAccount = await _accountRepository.GetByIdAsync(request.ToAccountId)
                ?? throw new AccountNotFoundException(request.ToAccountId);

            sourceAccount.Balance -= request.Amount;
            destinationAccount.Balance += request.Amount;

            var debitTransaction = new Transaction
            {
                AccountId = sourceAccount.Id,
                Type = TransactionType.Transfer,
                Amount = request.Amount,
                BalanceAfter = sourceAccount.Balance,
                Description = request.Description,
                RelatedAccountId = destinationAccount.Id
            };

            var creditTransaction = new Transaction
            {
                AccountId = destinationAccount.Id,
                Type = TransactionType.Transfer,
                Amount = request.Amount,
                BalanceAfter = destinationAccount.Balance,
                Description = request.Description,
                RelatedAccountId = sourceAccount.Id
            };

            await _transactionRepository.AddRangeAsync(new[] { debitTransaction, creditTransaction });
            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(destinationAccount);

            return MapToTransactionDto(debitTransaction);

        }
        public async Task<IEnumerable<TransactionResponseDto>> GetHistoryAsync(Guid accountId, Guid userId)
        {
            var account = await GetOwnedAccountAsync(accountId, userId);
            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
            var transactionDtos = new List<TransactionResponseDto>();
            foreach (var transaction in transactions)
            {
                {
                    transactionDtos.Add(MapToTransactionDto(transaction));
                }
            }
            return transactionDtos;
        }
        private async Task<Account> GetOwnedAccountAsync(Guid accountId, Guid userId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId)
                ?? throw new AccountNotFoundException(accountId);

            if (account.UserId != userId)
            {
                throw new UnauthorizedAccountAccessException();
            }
            return account;
        }

        private TransactionResponseDto MapToTransactionDto(Transaction transaction)
        {
            return new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                BalanceAfter = transaction.BalanceAfter,
                Description = transaction.Description,
                RelatedAccountId = transaction.RelatedAccountId,
                Timestamp = transaction.Timestamp
            };
        }
    }
}
