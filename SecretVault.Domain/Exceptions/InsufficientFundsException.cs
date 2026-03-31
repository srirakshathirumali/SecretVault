using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Exceptions
{
    public class InsufficientFundsException:Exception
    {
        public InsufficientFundsException()
        : base("Account balance is insufficient for this transaction.") { }
    }
}
