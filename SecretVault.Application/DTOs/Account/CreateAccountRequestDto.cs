using SecretVault.Domain.Enums;

namespace SecretVault.Application.DTOs.Account
{
    public class CreateAccountRequestDto
    {
        public AccountType Type { get; set; }
    }
}
