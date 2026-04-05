using Microsoft.EntityFrameworkCore;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;

namespace SecretVault.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(AuditLog auditLog)
        {
            auditLog.Id = Guid.NewGuid();
            auditLog.Timestamp = DateTime.UtcNow;
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<AuditLog>> GetAllAync()
        {
            return await _context.AuditLogs.OrderByDescending(a => a.Timestamp).ToListAsync();
        }
    }
}
