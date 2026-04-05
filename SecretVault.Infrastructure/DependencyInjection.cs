using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using SecretVault.Application.Interfaces;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;
using SecretVault.Infrastructure.Repositories;
using SecretVault.Infrastructure.Services;

namespace SecretVault.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SecureVault.Infrastructure")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            //Register services
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IS3Service, S3Service>();

            // Minio client
            services.AddMinio(config => config
                .WithEndpoint(
                    configuration["S3:Endpoint"] ?? "localhost",
                    int.Parse(configuration["S3:Port"] ?? "9000"))
                .WithCredentials(
                    configuration["S3:AccessKey"] ?? "minioadmin",
                    configuration["S3:SecretKey"] ?? "minioadmin")
                .WithSSL(false)
                .Build());

            return services;
        }

    }
}
