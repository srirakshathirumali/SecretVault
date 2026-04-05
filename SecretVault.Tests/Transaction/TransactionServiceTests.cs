using Moq;
using SecretVault.Application.DTOs.Transaction;
using SecretVault.Application.Services;
using SecretVault.Domain.Enums;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;

namespace SecretVault.Tests.Transaction
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly TransactionService _sut;
        public TransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _sut = new TransactionService(_transactionRepositoryMock.Object, _accountRepositoryMock.Object);
        }

        [Fact]
        public async Task DepositAsync_ShouldDepositAmount_WhenRequestIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new DepositRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test deposit"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                Balance = 0
            };

            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Transaction>())).Returns(Task.CompletedTask);

            _accountRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DepositAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(account.Balance, result.BalanceAfter);

        }

        [Fact]
        public async Task DepositAsync_ShouldNoDeposit_WhenAccountNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new DepositRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test deposit"
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Domain.Entities.Account?)null);
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.DepositAsync(request, userId));
        }

        [Fact]
        public async Task DepositAsync_ShouldNoDeposit_WhenAccountNotOwnedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new DepositRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test deposit"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 0
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccountAccessException>(() => _sut.DepositAsync(request, userId));
        }

        [Fact]
        public async Task WithdrawAsync_ShouldWithdrawAmount_WhenRequestIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new WithdrawRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test withdraw"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                Balance = 200
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Transaction>())).Returns(Task.CompletedTask);
            _accountRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.WithdrawAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(account.Balance, result.BalanceAfter);
        }

        [Fact]
        public async Task WithdrawAsync_ShouldNoWithdraw_WhenInsufficientFunds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new WithdrawRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test withdraw"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                Balance = 50 // Insufficient funds
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _sut.WithdrawAsync(request, userId));
        }

        [Fact]
        public async Task WithdrawAsync_ShouldNoWithdraw_WhenAccountNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new WithdrawRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test withdraw"
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Domain.Entities.Account?)null);
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.WithdrawAsync(request, userId));
        }

        [Fact]
        public async Task WithdrawAsync_ShouldNoWithdraw_WhenAccountNotOwnedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new WithdrawRequestDto
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test withdraw"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 200
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccountAccessException>(() => _sut.WithdrawAsync(request, userId));
        }

        [Fact]
        public async Task TransferAsync_ShouldTransferAmount_WhenRequestIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sourceAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = sourceAccountId,
                ToAccountId = destinationAccountId,
                Amount = 100,
                Description = "Test transfer"
            };
            var sourceAccount = new Domain.Entities.Account
            {
                Id = sourceAccountId,
                UserId = userId,
                Balance = 200
            };
            var destinationAccount = new Domain.Entities.Account
            {
                Id = destinationAccountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 50
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(sourceAccountId)).ReturnsAsync(sourceAccount);
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(destinationAccountId)).ReturnsAsync(destinationAccount);
            _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Transaction>())).Returns(Task.CompletedTask);
            _accountRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Account>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.TransferAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(sourceAccount.Balance, result.BalanceAfter);
        }

        [Fact]
        public async Task TransferAsync_ShouldNoTransfer_WhenInsufficientFunds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sourceAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = sourceAccountId,
                ToAccountId = destinationAccountId,
                Amount = 100,
                Description = "Test transfer"
            };
            var sourceAccount = new Domain.Entities.Account
            {
                Id = sourceAccountId,
                UserId = userId,
                Balance = 50 // Insufficient funds
            };
            var destinationAccount = new Domain.Entities.Account
            {
                Id = destinationAccountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 50
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(sourceAccountId)).ReturnsAsync(sourceAccount);
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(destinationAccountId)).ReturnsAsync(destinationAccount);

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _sut.TransferAsync(request, userId));
        }

        [Fact]
        public async Task TransferAsync_ShouldNoTransfer_WhenSourceAccountNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sourceAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = sourceAccountId,
                ToAccountId = destinationAccountId,
                Amount = 100,
                Description = "Test transfer"
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(sourceAccountId)).ReturnsAsync((Domain.Entities.Account?)null);

            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.TransferAsync(request, userId));
        }

        [Fact]
        public async Task TransferAsync_ShouldNotTransfer_WhenSourceAccountNotAuothrized()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sourceAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = sourceAccountId,
                ToAccountId = destinationAccountId,
                Amount = 100,
                Description = "Test transfer"
            };
            var sourceAccount = new Domain.Entities.Account
            {
                Id = sourceAccountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 200
            };

            _accountRepositoryMock.Setup(r => r.GetByIdAsync(sourceAccountId)).ReturnsAsync(sourceAccount);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccountAccessException>(() => _sut.TransferAsync(request, userId));
        }

        [Fact]
        public async Task TransferAsync_ShouldNotTransfer_WhenSourceAndDestinationAccountsAreSame()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = accountId,
                ToAccountId = accountId, // Same account
                Amount = 100,
                Description = "Test transfer"
            };
            var account = new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                Balance = 200
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);
            // Act & Assert
            await Assert.ThrowsAsync<SelfTransferException>(() => _sut.TransferAsync(request, userId));
        }

        [Fact]
        public async Task TransferAsync_ShouldNotTransfer_WhenDestinationAccountNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sourceAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var request = new TransferRequestDto
            {
                FromAccountId = sourceAccountId,
                ToAccountId = destinationAccountId,
                Amount = 100,
                Description = "Test transfer"
            };
            var sourceAccount = new Domain.Entities.Account
            {
                Id = sourceAccountId,
                UserId = userId,
                Balance = 200
            };
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(sourceAccountId)).ReturnsAsync(sourceAccount);
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(destinationAccountId)).ReturnsAsync((Domain.Entities.Account?)null);
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.TransferAsync(request, userId));
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldReturnTransactions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var transactions = new List<Domain.Entities.Transaction>
            {
                new Domain.Entities.Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    Type = TransactionType.Deposit,
                    Amount = 100,
                    BalanceAfter = 100,
                    Description = "Test deposit",
                    Timestamp = DateTime.UtcNow
                },
                new Domain.Entities.Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    Type = TransactionType.Withdraw,
                    Amount = 50,
                    BalanceAfter = 50,
                    Description = "Test withdraw",
                    Timestamp = DateTime.UtcNow.AddMinutes(-1)
                }
            };

            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(new Domain.Entities.Account
            {
                Id = accountId,
                UserId = userId,
                Balance = 50
            });
            _transactionRepositoryMock.Setup(r => r.GetByAccountIdAsync(accountId)).ReturnsAsync(transactions);

            // Act
            var result = await _sut.GetHistoryAsync(accountId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldNoReturnTransactions_WhenAccountNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Domain.Entities.Account?)null);
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _sut.GetHistoryAsync(accountId, userId));
        }

        [Fact]
        public async Task GetHistoryAsync_ShouldNoReturnTransactions_WhenAccountNotOwnedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(new Domain.Entities.Account
            {
                Id = accountId,
                UserId = Guid.NewGuid(), // Different user
                Balance = 50
            });
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccountAccessException>(() => _sut.GetHistoryAsync(accountId, userId));
        }
    }
}
