namespace SecretVault.Domain.Exceptions
{
    public class InsufficientFundException : Exception
    {
        public InsufficientFundException()
            : base("Account balance is insufficient for this transaction.")
        {
        }
    }
}
