using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SecretVault.API.Middleware;
using SecretVault.Application;
using SecretVault.Infrastructure;
using SecretVaultAPI.Middleware;
using Serilog;
using System.Text;

// Configure Serilog first — before anything else
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/securevault-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddInfrastructure(builder.Configuration);

    // Rate limiting
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(
        builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(
        builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration,
        RateLimitConfiguration>();

    builder.Services.AddApplication();

    //Add JWT authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
            };
        });


    // Services section
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Info = new()
            {
                Title = "SecureVault API",
                Version = "v1",
                Description = "Clean Architecture banking API — ASP.NET Core 8, JWT, EF Core, AWS S3"
            };
            return Task.CompletedTask;
        });
    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // GlobalExceptionMiddleware MUST be first
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "SecureVault API";
            options.Theme = ScalarTheme.DeepSpace;
        });
    }
    app.UseIpRateLimiting();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<AuditMiddleware>();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
