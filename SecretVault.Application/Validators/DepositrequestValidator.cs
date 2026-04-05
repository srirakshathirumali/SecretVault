using FluentValidation;
using SecretVault.Application.DTOs.Transaction;

namespace SecretVault.Application.Validators
{
    public class DepositrequestValidator : AbstractValidator<DepositRequestDto>
    {
        public DepositrequestValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty().WithMessage("AccountId is required.");
            RuleFor(x => x.Amount)
            .GreaterThan(0.01m).WithMessage("Deposit amount must be greater than $0.01.");
            RuleFor(x => x.Description).MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
