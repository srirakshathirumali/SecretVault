using SecretVault.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using BCrypt.Net;

namespace SecretVault.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
