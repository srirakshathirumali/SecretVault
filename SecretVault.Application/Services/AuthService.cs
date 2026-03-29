using SecretVault.Application.DTOs;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Exceptions;
using SecretVault.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecretVault.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        public AuthService(IUserRepository userRepository, IPasswordService passwordService, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                bool emailExists =await _userRepository.EmailExistsAsync(request.Email);
                if (emailExists)
                {
                    throw new EmailAlreadyExistsException(request.Email);
                }
                User user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = _passwordService.Hash(request.Password),
                };
                await _userRepository.AddAync(user);
                return new RegisterResponseDto
                {
                    UserId = user.Id,
                    Message = "User registered successfully"
                };
                
            }
            catch (EmailAlreadyExistsException ex)
            {
                return new RegisterResponseDto
                {
                    Message = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponseDto
                {
                    Message = $"An error occurred during registration: {ex.Message}",
                };
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
           var user= await _userRepository.GetUserByEmailAsync(request.Email);
           if(user == null || !_passwordService.Verify(request.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException();
            }
            
            var accessToken = _jwtService.GenerateAccessToken(user);
            user.RefreshToken = HashRefreshToken(_jwtService.GenerateRefreshToken());
            user.TokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAync(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken,
                AccessTokenExpiry = _jwtService.GetAccessTokenExpiry(),
                Email=user.Email,
                FullName=user.FullName
            };
        }

        // Hash refresh token before storing — raw token never saved to DB
        private static string HashRefreshToken(string token)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
