using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AIHubTaskTracker.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AIHubTaskTracker.Middleware
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext db)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();
                var now = DateTime.UtcNow;

                bool isBlacklisted = await db.BlacklistedTokens
                    .AnyAsync(t => t.Token == token && t.ExpiresAt > now);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token đã bị thu hồi." });
                    return;
                }
            }

            await _next(context);
        }
    }
}
