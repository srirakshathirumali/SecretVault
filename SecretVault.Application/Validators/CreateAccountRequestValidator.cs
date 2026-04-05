using FluentValidation;
using SecretVault.Application.DTOs.Account;

namespace SecretVault.Application.Validators
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequestDto>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Account Type must be either Savings(0) or Checking(1).");
        }
    }
}
