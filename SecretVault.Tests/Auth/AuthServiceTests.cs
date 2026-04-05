using Moq;
using SecretVault.Application.DTOs.Auth;
using SecretVault.Application.Interfaces;
using SecretVault.Application.Services;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;

namespace SecretVault.Tests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthService _sut;  //System under test
        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _sut = new AuthService(_userRepositoryMock.Object, _passwordServiceMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "testuser",
                Password = "Test@1234",
                FullName = "Test User"
            };
            _userRepositoryMock.Setup(repo => repo.EmailExistsAsync(request.Email))
                .ReturnsAsync(false); // No existing user

            _passwordServiceMock.Setup(service => service.Hash(request.Password))
                .Returns("hashedpassword");

            _userRepositoryMock.Setup(repo => repo.AddAync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.UserId.ToString());
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "testuser",
                Password = "Test@1234",
                FullName = "Test User"
            };
            _userRepositoryMock.Setup(repo => repo.EmailExistsAsync(request.Email))
                .ReturnsAsync(true); // Email already exists
            // Act & Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _sut.RegisterAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "testuser",
                Password = "Test@1234"
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedpassword",
                FullName = "Test User",
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(service => service.Verify(request.Password, user.PasswordHash))
                .Returns(true);
            _jwtServiceMock.Setup(service => service.GenerateAccessToken(user))
                .Returns("valid.jwt.token");
            _jwtServiceMock.Setup(service => service.GenerateRefreshToken())
                .Returns("valid.refresh.token");
            _userRepositoryMock.Setup(repo => repo.UpdateAync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("valid.jwt.token", result.AccessToken);
            Assert.Equal(user.FullName, result.FullName);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "nonexistentuser",
                Password = "Test@1234"
            };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User?)null); // User not found

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "testuser",
                Password = "WrongPassword"
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedpassword",
                FullName = "Test User",
            };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(service => service.Verify(request.Password, user.PasswordHash))
                .Returns(false); // Incorrect password
            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request));
        }
    }
}
