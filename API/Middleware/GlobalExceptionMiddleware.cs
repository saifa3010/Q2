using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions and maps them to consistent ProblemDetails responses.
    /// Domain rule violations (ArgumentException, InvalidOperationException) → 400/409.
    /// Not found (KeyNotFoundException) → 404.
    /// Everything else → 500.
    /// </summary>
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                await WriteErrorResponseAsync(context, ex);
            }
        }

        private static Task WriteErrorResponseAsync(HttpContext context, Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                KeyNotFoundException    => (HttpStatusCode.NotFound,           "Resource not found"),
                ArgumentException       => (HttpStatusCode.BadRequest,         "Invalid request"),
                InvalidOperationException { Message: var msg }
                    when msg.Contains("cannot", StringComparison.OrdinalIgnoreCase)
                                        => (HttpStatusCode.Conflict,           "Business rule violation"),
                InvalidOperationException => (HttpStatusCode.BadRequest,       "Invalid operation"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized,   "Unauthorized"),
                _                       => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode  = (int)statusCode;

            var problem = new
            {
                type     = $"https://httpstatuses.com/{(int)statusCode}",
                title,
                status   = (int)statusCode,
                detail   = statusCode == HttpStatusCode.InternalServerError
                               ? "An internal error occurred. Please try again later."
                               : ex.Message,
                traceId  = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(problem, _jsonOptions);
            return context.Response.WriteAsync(json);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
