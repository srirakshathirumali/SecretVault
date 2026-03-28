using SecretVault.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash {  get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedOn {  get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }

        //Navigation
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    }
}
