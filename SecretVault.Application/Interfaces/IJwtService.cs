using SecretVault.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiry();
    }
}
