using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.Exceptions;
using System.Net;
using System.Text;

namespace QuickDelivery.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. RequestPath: {RequestPath}, Method: {Method}, User: {User}",
                    context.Request.Path, context.Request.Method, context.User?.Identity?.Name ?? "Anonymous");

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var apiResponse = CreateApiResponse(exception);
            response.StatusCode = apiResponse.StatusCode;

            var jsonResponse = JsonConvert.SerializeObject(apiResponse, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = _environment.IsDevelopment() ? Formatting.Indented : Formatting.None
            });

            await response.WriteAsync(jsonResponse, Encoding.UTF8);
        }

        private ErrorApiResponse CreateApiResponse(Exception exception)
        {
            return exception switch
            {
                BaseException baseEx => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = baseEx.StatusCode,
                    ErrorCode = baseEx.ErrorCode,
                    Message = baseEx.Message,
                    Details = baseEx.Details,
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? baseEx.ToString() : null
                },

                TaskCanceledException => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 408,
                    ErrorCode = "REQUEST_TIMEOUT",
                    Message = "The request timed out.",
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                OperationCanceledException => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 408,
                    ErrorCode = "REQUEST_TIMEOUT",
                    Message = "The request was cancelled.",
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                UnauthorizedAccessException => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 401,
                    ErrorCode = "UNAUTHORIZED",
                    Message = "Unauthorized access.",
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                ArgumentException argEx => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_ARGUMENT",
                    Message = argEx.Message,
                    Details = new { Parameter = argEx.ParamName },
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                InvalidOperationException => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_OPERATION",
                    Message = exception.Message,
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                NotImplementedException => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 501,
                    ErrorCode = "NOT_IMPLEMENTED",
                    Message = "This functionality is not yet implemented.",
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                },

                _ => new ErrorApiResponse
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred. Please try again later.",
                    Timestamp = DateTime.UtcNow,
                    DeveloperMessage = _environment.IsDevelopment() ? exception.ToString() : null
                }
            };
        }
    }

    // Extension method pentru a înregistra middleware-ul mai ușor
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}