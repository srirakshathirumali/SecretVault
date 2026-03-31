using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Exceptions
{
    public class SelfTransferException:Exception
    {
        public SelfTransferException() : base("Cannot Transfer to the same account.")
        {
        
        }
    }
}
