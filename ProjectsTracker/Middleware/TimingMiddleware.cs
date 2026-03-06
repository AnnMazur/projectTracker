
using System.Diagnostics;

namespace ProjectsTracker.Middleware;
public class TimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TimingMiddleware> _logger;

    public TimingMiddleware(RequestDelegate next, ILogger<TimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            context.Items["RequestDuration"] = sw.ElapsedMilliseconds;

            var requestId = context.Items["RequestId"]?.ToString() ?? "N/A";
            _logger.LogInformation(
                "Request {RequestId} completed in {Duration}ms with status {StatusCode}",
                requestId,
                sw.ElapsedMilliseconds,
                context.Response.StatusCode);
        }
    }
}