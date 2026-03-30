using SecretVault.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.DTOs.Account
{
    public class AccountResponseDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
