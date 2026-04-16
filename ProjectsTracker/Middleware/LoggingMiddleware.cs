using ProjectsTracker.Config;
using System.Diagnostics;

namespace ProjectsTracker.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly AppConfig _config;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, AppConfig config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
        context.Items["RequestId"] = requestId;

        if (_config.Mode.DetailedLogging)
        {
            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path}{QueryString} started from {IP}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Connection.RemoteIpAddress);
        }

        await _next(context);
    }
}

