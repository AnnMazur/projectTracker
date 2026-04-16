// Middleware/TrustedOriginsMiddleware.cs - исправленная версия
using ProjectsTracker.Config;

namespace ProjectsTracker.Middleware;

public class TrustedOriginsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TrustedOriginsMiddleware> _logger;
    private readonly AppConfig _config;

    public TrustedOriginsMiddleware(RequestDelegate next, ILogger<TrustedOriginsMiddleware> logger, AppConfig config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_config.Security.EnableCors && _config.Security.TrustedOrigins.Any())
        {
            var origin = context.Request.Headers.Origin.ToString();

            if (!string.IsNullOrEmpty(origin) && !IsTrustedOrigin(origin))
            {
                _logger.LogWarning("Blocked request from untrusted origin: {Origin}", origin);

                context.Response.StatusCode = 403;

                object response;
                if (_config.Mode.VerboseErrors)
                {
                    response = new ForbiddenErrorResponse
                    {
                        Error = "Forbidden",
                        Message = $"Origin '{origin}' is not trusted"
                    };
                }
                else
                {
                    response = new ForbiddenErrorResponse
                    {
                        Error = "Forbidden"
                    };
                }

                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }

        await _next(context);
    }

    private bool IsTrustedOrigin(string origin)
    {
        return _config.Security.TrustedOrigins.Any(to =>
            origin.Equals(to, StringComparison.OrdinalIgnoreCase));
    }
}

// DTO для ответа forbidden
public class ForbiddenErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Message { get; set; }
}