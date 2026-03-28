using System;
using System.Collections.Generic;
using System.Text;
using AuditLog= SecretVault.Domain.Entities.AuditLog;

namespace SecretVault.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAync();
    }
}
