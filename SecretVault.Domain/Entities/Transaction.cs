using SecretVault.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public DateTime Timestamp { get; set; }

        //Navigation
        public Account Account { get; set; } = null!;

    }
}
