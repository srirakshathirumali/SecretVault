using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string hashedPassword);
    }
}
