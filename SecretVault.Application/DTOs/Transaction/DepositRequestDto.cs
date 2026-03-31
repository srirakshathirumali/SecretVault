using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.DTOs.Transaction
{
    public class DepositRequestDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
