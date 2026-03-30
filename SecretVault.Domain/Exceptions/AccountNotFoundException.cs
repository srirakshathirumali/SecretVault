using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Exceptions
{
    public class AccountNotFoundException:Exception
    {
        public AccountNotFoundException(Guid accountId) : base($"The account with ID '{accountId}' was not found.")
        {
        }
    }
}
