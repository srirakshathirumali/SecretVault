using FluentValidation;
using SecretVault.Application.DTOs.Transaction;

namespace SecretVault.Application.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromAccountId)
         .NotEmpty().WithMessage("Source account is required.");

            RuleFor(x => x.ToAccountId)
                .NotEmpty().WithMessage("Destination account is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0.01m).WithMessage("Transfer amount must be greater than $0.01.")
                .LessThanOrEqualTo(10000m).WithMessage("Single transfer cannot exceed $10,000.");

            RuleFor(x => x)
                .Must(x => x.FromAccountId != x.ToAccountId)
                .WithMessage("Cannot transfer to the same account.");

            RuleFor(x => x.Description)
                .MaximumLength(200).When(x => x.Description != null)
                .WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
