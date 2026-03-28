using SecretVault.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }

        //Navigation
        public User User { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; }= new List<Transaction>();
    }
}
