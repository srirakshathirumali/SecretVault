using Microsoft.Extensions.Configuration.EnvironmentVariables;
using SecretVault.Application.DTOs.Account;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Enums;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Transactions;

namespace SecretVault.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IS3Service _s3Service;
        public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IS3Service s3Service)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _s3Service = s3Service;
        }
        public async Task<AccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request, Guid userId)
        {
            Account account = new Account
            {
                UserId = userId,
                AccountNumber = await GenerateUniqueAccountNumber(),
                Balance = 0,
                Type = request.Type,
                IsActive = true,
            };

            await _accountRepository.AddAsync(account);

            return MapToDto(account);
        }

        public async Task<AccountResponseDto> GetAccountByAccountId(Guid accountId, Guid userId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            
            if (account == null)
                throw new AccountNotFoundException(accountId);

            if (account.UserId != userId)
                throw new UnauthorizedAccountAccessException();

            return MapToDto(account);
        }

        public async Task<IEnumerable<AccountResponseDto>> GetUserAccountsAsync(Guid userId)
        {
            var accounts =await _accountRepository.GetByUserIdAsync(userId);
            List<AccountResponseDto> response = new List<AccountResponseDto>();
            foreach (var account in accounts)
            {
                response.Add(MapToDto(account));
            }
            return response;
        }
         public async Task<string> GetStatementUrlAsync(Guid accountId, Guid userId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if(account == null)
                throw new AccountNotFoundException(accountId);

            if (account.UserId != userId)
                throw new UnauthorizedAccountAccessException();

            var month = DateTime.UtcNow.Month;
            var year = DateTime.UtcNow.Year;

            var statementExists = await _s3Service.StatementExistsAsync(accountId, month, year);

            if (!statementExists)
            {
                var content = await GenerateStatementContentAsync(account, month, year);
                await _s3Service.UploadStatementAsync(accountId,content, month, year);
            }
            return await _s3Service.GetPreSignedUrlAsync(accountId, month, year);

        }
        private async Task<string> GenerateStatementContentAsync(Account account,int month, int year)
        {
            var transactions = await _transactionRepository
            .GetByAccountIdAsync(account.Id);

            var monthlyTxns = transactions
                .Where(t => t.Timestamp.Year == year &&
                            t.Timestamp.Month == month)
                .OrderBy(t => t.Timestamp)
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("================================================");
            sb.AppendLine("         SECUREVAULT BANK STATEMENT             ");
            sb.AppendLine("================================================");
            sb.AppendLine($"Account Number : {account.AccountNumber}");
            sb.AppendLine($"Account Type   : {account.Type}");
            sb.AppendLine($"Statement Date : {new DateTime(year, month, 1):MMMM yyyy}");
            sb.AppendLine($"Current Balance: ${account.Balance:F2}");
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine("Date                Type          Amount    Balance");
            sb.AppendLine("------------------------------------------------");

            foreach (var txn in monthlyTxns)
            {
                sb.AppendLine(
                    $"{txn.Timestamp:yyyy-MM-dd HH:mm}  " +
                    $"{txn.Type,-12}  " +
                    $"${txn.Amount,8:F2}  " +
                    $"${txn.BalanceAfter:F2}");
            }

            sb.AppendLine("------------------------------------------------");
            sb.AppendLine($"Total Transactions : {monthlyTxns.Count}");
            sb.AppendLine("================================================");

            return sb.ToString();
        }
        private async Task<string> GenerateUniqueAccountNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string accountNumber;
            do
            {
                accountNumber = new string(Enumerable.Repeat(chars, 10)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (await _accountRepository.AccountNumberExistsAsync(accountNumber));
            return accountNumber;
        }
        private AccountResponseDto MapToDto(Account account)
        {
            return new AccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Type = account.Type,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedOn = account.CreatedOn
            };
        }
    }
}
