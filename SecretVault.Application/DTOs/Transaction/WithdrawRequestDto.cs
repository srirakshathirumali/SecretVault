namespace SecretVault.Application.DTOs.Transaction
{
    public class WithdrawRequestDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
