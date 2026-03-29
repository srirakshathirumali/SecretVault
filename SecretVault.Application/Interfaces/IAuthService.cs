using SecretVault.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
