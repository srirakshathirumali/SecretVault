using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Exceptions
{
    public class InvalidCredentialsException: Exception
    {
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }
    }
}
