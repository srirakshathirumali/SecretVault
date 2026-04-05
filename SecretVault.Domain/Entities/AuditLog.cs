namespace SecretVault.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? IPAddress { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
