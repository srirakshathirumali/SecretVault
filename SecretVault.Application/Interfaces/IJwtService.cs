using SecretVault.Domain.Entities;

namespace SecretVault.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiry();
    }
}
