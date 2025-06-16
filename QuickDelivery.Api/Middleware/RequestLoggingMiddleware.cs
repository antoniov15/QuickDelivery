using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace QuickDelivery.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Loghează începutul cererii
            _logger.LogInformation(
                "Request started: {Method} {Path}{QueryString}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);

            var stopwatch = Stopwatch.StartNew();

            // Permitem citirea corpului cererii
            context.Request.EnableBuffering();

            // Citim corpul cererii pentru logging
            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
            {
                using (var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    // Resetăm poziția pentru cititorii următori
                    context.Request.Body.Position = 0;
                }

                if (!string.IsNullOrEmpty(requestBody))
                {
                    // Trunchiază corpul pentru loggingul de bază
                    var truncatedBody = requestBody.Length > 1000 ? $"{requestBody.Substring(0, 1000)}..." : requestBody;
                    _logger.LogInformation("Request body: {RequestBody}", truncatedBody);
                }
            }

            // Captăm răspunsul original
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);

                // După ce s-a procesat cererea, citim răspunsul
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);

                if (!string.IsNullOrEmpty(responseBody) && context.Response.ContentType?.Contains("application/json") == true)
                {
                    // Trunchiază dacă e prea lung
                    var truncatedResponse = responseBody.Length > 1000 ? $"{responseBody.Substring(0, 1000)}..." : responseBody;
                    _logger.LogInformation("Response body: {ResponseBody}", truncatedResponse);
                }

                // Copiem înapoi pe stream-ul original
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                context.Response.Body = originalBodyStream;

                // Loghează sfârșitul cererii cu durata și codul de status
                _logger.LogInformation(
                    "Request completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }

    // Extensie pentru a adăuga middleware-ul la pipeline-ul HTTP
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}