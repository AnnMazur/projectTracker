using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProjectsTracker.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.Items["RequestId"]?.ToString() ?? "N/A";

        _logger.LogError(exception, "Request {RequestId} failed: {Message}", requestId, exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, errorCode, message) = exception switch
        {
            KeyNotFoundException => (404, "NOT_FOUND", exception.Message),
            ValidationException => (400, "VALIDATION_ERROR", "Validation failed"),
            InvalidOperationException => (409, "CONFLICT", "Operation conflict"),
            _ => (500, "INTERNAL_ERROR", "An internal error occurred")
        };

        response.StatusCode = statusCode;

        var error = new
        {
            statusCode,
            errorCode,
            message,
            requestId,
            timestamp = DateTime.UtcNow
        };

        await response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
