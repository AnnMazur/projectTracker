using ProjectsTracker.Config;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProjectsTracker.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly AppConfig _config;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, AppConfig config)
    {
        _next = next;
        _logger = logger;
        _config = config;
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

        if (_config.Mode.DetailedLogging || _config.Mode.Environment == "Development")
        {
            _logger.LogError(exception, "Request {RequestId} failed: {Message}", requestId, exception.Message);
        }
        else
        {
            _logger.LogError("Request {RequestId} failed with {ExceptionType}", requestId, exception.GetType().Name);
        }

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

        object error;

        if (_config.Mode.VerboseErrors)
        {
            error = new
            {
                statusCode,
                errorCode,
                message,
                requestId,
                timestamp = DateTime.UtcNow,
                detail = exception.Message
            };
        }
        else
        {
            error = new
            {
                statusCode,
                errorCode,
                message = _config.Mode.Environment == "Production" ? "An error occurred" : message,
                requestId
            };
        }

        await response.WriteAsync(JsonSerializer.Serialize(error));
    }
}