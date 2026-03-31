using SecretVault.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Exceptions
{
    public class InsufficientFundException:Exception
    {
        public InsufficientFundException() 
            : base("Account balance is insufficient for this transaction.")
        {
        }
    }
}
