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
        public AuthService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
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
    }
}
