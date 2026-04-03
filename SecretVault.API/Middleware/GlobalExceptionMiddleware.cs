using SecretVault.API.Models;
using SecretVault.Domain.Exceptions;
using Serilog;
using System.Net;
using System.Text.Json;

namespace SecretVaultAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = MapException(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Status = (int)statusCode,
            ErrorCode = errorCode,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode, string, string) MapException(Exception ex) =>
        ex switch
        {
            // 400 Bad Request
            ArgumentException e =>
                (HttpStatusCode.BadRequest,
                 "BAD_REQUEST", e.Message),

            // 401 Unauthorized
            InvalidCredentialsException e =>
                (HttpStatusCode.Unauthorized,
                 "INVALID_CREDENTIALS", e.Message),

            // 403 Forbidden
            UnauthorizedAccountAccessException e =>
                (HttpStatusCode.Forbidden,
                 "FORBIDDEN", e.Message),

            // 404 Not Found
            AccountNotFoundException e =>
                (HttpStatusCode.NotFound,
                 "NOT_FOUND", e.Message),

            // 409 Conflict — email exists
            EmailAlreadyExistsException e =>
                (HttpStatusCode.Conflict,
                 "EMAIL_EXISTS", e.Message),

            // 409 Conflict — insufficient funds
            InsufficientFundsException e =>
                (HttpStatusCode.Conflict,
                 "INSUFFICIENT_FUNDS", e.Message),

            // 422 Unprocessable Entity
            SelfTransferException e =>
                (HttpStatusCode.UnprocessableEntity,
                 "INVALID_OPERATION", e.Message),

            // 500 — everything else
            _ =>
                (HttpStatusCode.InternalServerError,
                 "INTERNAL_ERROR",
                 "An unexpected error occurred.")
        };
}