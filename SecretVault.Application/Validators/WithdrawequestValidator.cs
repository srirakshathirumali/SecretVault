using FluentValidation;
using SecretVault.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Validators
{
    public class WithdrawequestValidator:AbstractValidator<WithdrawRequestDto>
    {
        public WithdrawequestValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty().WithMessage("AccountId is required.");
            RuleFor(x => x.Amount)
            .GreaterThan(0.01m).WithMessage("Withdrawal amount must be greater than $0.01.")
            .LessThanOrEqualTo(10000m).WithMessage("Single withdrawal cannot exceed $10,000.");
            RuleFor(x => x.Description).MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
