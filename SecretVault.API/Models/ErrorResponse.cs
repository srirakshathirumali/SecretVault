namespace SecretVault.API.Models
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
