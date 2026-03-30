using FluentValidation;
using SecretVault.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Validators
{
    public class LoginRequestValidator:AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
 }
