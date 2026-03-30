using Microsoft.Extensions.Configuration.EnvironmentVariables;
using SecretVault.Application.DTOs.Account;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Enums;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
