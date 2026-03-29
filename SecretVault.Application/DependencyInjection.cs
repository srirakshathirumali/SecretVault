using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SecretVault.Application.Interfaces;
using SecretVault.Application.Services;
using SecretVault.Application.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            return services;
        }
    }
}
