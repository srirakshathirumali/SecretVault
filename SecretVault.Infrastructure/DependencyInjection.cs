using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;
using SecretVault.Infrastructure.Repositories;
using SecretVault.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b=>b.MigrationsAssembly("SecureVault.Infrastructure")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            //Register services
            services.AddScoped<IPasswordService, PasswordService>();

            return services;
        }

    }
}
