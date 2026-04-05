using SecretVault.Domain.Entities;
using SecretVault.Domain.Interfaces;
using System.Security.Claims;

namespace SecretVault.API.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
        {
            var method = context.Request.Method;

            if (method != HttpMethods.Post && method != HttpMethods.Put && method != HttpMethods.Delete && method != HttpMethods.Patch)
            {
                await _next(context);
                return;
            }

            await _next(context);

            if (context.Response.StatusCode >= 400)
                return;

            try
            {
                var userId = GetUserId(context);
                var ipAddress = GetIpAddress(context);
                var action = BuildAction(context);
                var entityType = BuildEntityType(context);

                var log = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    IPAddress = ipAddress,
                    Details = $"{method} {context.Request.Path}"
                };

                await auditLogRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here) but don't disrupt the user experience
            }
        }

        private static Guid? GetUserId(HttpContext context)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim is not null ? Guid.Parse(claim) : null;
        }

        private static string GetIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (behind proxy/load balancer)
            var forwarded = context.Request.Headers["X-Forwarded-For"]
                                   .FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private static string BuildAction(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? string.Empty;

            // Build a readable action name from method + path
            // e.g. POST /api/transactions/deposit → DEPOSIT
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var last = segments.LastOrDefault() ?? "UNKNOWN";

            return method switch
            {
                "POST" => last.ToUpper(),
                "PUT" => $"UPDATE_{last.ToUpper()}",
                "DELETE" => $"DELETE_{last.ToUpper()}",
                "PATCH" => $"PATCH_{last.ToUpper()}",
                _ => $"{method}_{last.ToUpper()}"
            };
        }

        private static string BuildEntityType(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // /api/accounts → Accounts
            // /api/transactions/deposit → Transactions
            return segments.Length >= 2
                ? char.ToUpper(segments[1][0]) + segments[1][1..]
                : "Unknown";
        }

    }
}
