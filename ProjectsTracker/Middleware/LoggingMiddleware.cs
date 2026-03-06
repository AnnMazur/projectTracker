using System.Diagnostics;

namespace ProjectsTracker.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
        context.Items["RequestId"] = requestId;

        _logger.LogInformation(
            "Request {RequestId}: {Method} {Path}{QueryString} started",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);

        await _next(context);
    }
}

