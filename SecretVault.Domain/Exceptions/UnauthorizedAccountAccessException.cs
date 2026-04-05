namespace SecretVault.Domain.Exceptions
{
    public class UnauthorizedAccountAccessException : Exception
    {
        public UnauthorizedAccountAccessException() : base("You do not have access to this account")
        {
        }
    }
}
