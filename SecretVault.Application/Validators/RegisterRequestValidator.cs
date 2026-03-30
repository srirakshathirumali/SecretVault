using FluentValidation;
using SecretVault.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Validators
{
    public class RegisterRequestValidator:AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Minimum 8 characters.")
                .Matches("[A-Z]").WithMessage("Must contain one uppercase letter.")
                .Matches("[0-9]").WithMessage("Must contain one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain one special character.");
        }
    }
}
