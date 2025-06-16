using Microsoft.AspNetCore.Http;
using QuickDelivery.Core.Interfaces.Services;
using System.Net;
using System.Text.Json;

namespace QuickDelivery.Api.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    var principal = tokenService.ValidateToken(token);
                    if (principal != null)
                    {
                        context.User = principal;
                    }
                }
                catch
                {
                    // Token invalid, continua fluxul
                }
            }

            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}