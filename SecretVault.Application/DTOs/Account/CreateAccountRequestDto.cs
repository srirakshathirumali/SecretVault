using SecretVault.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.DTOs.Account
{
    public class CreateAccountRequestDto
    {
        public AccountType Type { get; set; }
    }
}
