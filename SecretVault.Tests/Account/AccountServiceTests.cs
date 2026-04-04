using Moq;
using SecretVault.Application.DTOs;
using SecretVault.Application.DTOs.Account;
using SecretVault.Application.Interfaces;
using SecretVault.Application.Services;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Enums;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Tests.Account
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly AccountService _sut;
        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _s3ServiceMock = new Mock<IS3Service>();
            _sut = new AccountService(_accountRepositoryMock.Object, _transactionRepositoryMock.Object, _s3ServiceMock.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldCreateAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateAccountRequestDto
            {
                Type = AccountType.Checking
            };

            _accountRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Account>())).
                Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateAccountAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(AccountType.Checking, result.Type);
        }

        [Fact]
        public async Task GetAccountByAccountId_ShouldGetAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                AccountNumber = "1234567890",
                Balance = 1000,
                Type = AccountType.Savings,
                IsActive = true
            };

            _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act
            var result = await _sut.GetAccountByAccountId(accountId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.Id);
        }

        [Fact]
        public async Task GetUserAccountsAsync_ShouldReturnAccounts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            List<Domain.Entities.Account> accounts = new List<Domain.Entities.Account>
            {
                new Domain.Entities.Account
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    AccountNumber = "1234567890",
                    Balance = 1000,
                    Type = AccountType.Savings,
                    IsActive = true
                },
                new Domain.Entities.Account
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    AccountNumber = "0987654321",
                    Balance = 2000,
                    Type = AccountType.Checking,
                    IsActive = true
                }
            };

            _accountRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(accounts);

            // Act
            var result = await _sut.GetUserAccountsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetStatementsUrlAsync_ShouldReturnAccountStatements_StatementsAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(new Domain.Entities.Account
                {
                    Id = accountId,
                    UserId = userId,
                    AccountNumber = "1234567890",
                    Balance = 1000,
                    Type = AccountType.Savings,
                    IsActive = true
                });

            _s3ServiceMock.Setup(s3 => s3.StatementExistsAsync(accountId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            _s3ServiceMock.Setup(s3 => s3.GetPreSignedUrlAsync(accountId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync("https://example.com/statement.pdf");

            // Act
            var result = await _sut.GetStatementUrlAsync(accountId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://example.com/statement.pdf", result);
        }

        [Fact]
        public async Task GetStatementsUrlAsync_ShouldReturnAccountStatements_StatementsDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(new Domain.Entities.Account
                {
                    Id = accountId,
                    UserId = userId,
                    AccountNumber = "1234567890",
                    Balance = 1000,
                    Type = AccountType.Savings,
                    IsActive = true
                });

            _s3ServiceMock.Setup(s3 => s3.StatementExistsAsync(accountId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            _transactionRepositoryMock.Setup(repo => repo.GetByAccountIdAsync(accountId))
                .ReturnsAsync(new List<Domain.Entities.Transaction>
                {
                    new Domain.Entities.Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        Amount = 100,
                        Type = TransactionType.Deposit,
                        Timestamp = DateTime.UtcNow.AddDays(-10)
                    },
                    new Domain.Entities.Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        Amount = -50,
                        Type = TransactionType.Withdraw,
                        Timestamp = DateTime.UtcNow.AddDays(-5)
                    }
                });
            _s3ServiceMock.Setup(s3 => s3.UploadStatementAsync(accountId, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync("key");
            _s3ServiceMock.Setup(s3 => s3.GetPreSignedUrlAsync(accountId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync("https://example.com/statement.pdf");

            // Act
            var result = await _sut.GetStatementUrlAsync(accountId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://example.com/statement.pdf", result);
        }

        [Fact]
        public async Task GetStatementsUrlAsync_ShouldThrowAccountNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync((Domain.Entities.Account?)
                null);
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.GetStatementUrlAsync(accountId, userId));
        }

        [Fact]
        public async Task GetStatementsUrlAsync_ShouldThrowUnauthorizedAccountAccessException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(new Domain.Entities.Account
                {
                    Id = accountId,
                    UserId = anotherUserId,
                    AccountNumber = "1234567890",
                    Balance = 1000,
                    Type = AccountType.Savings,
                    IsActive = true
                });
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccountAccessException>(() => _sut.GetStatementUrlAsync(accountId, userId));
        }
    }
}
