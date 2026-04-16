using ProjectsTracker.Config;

namespace ProjectsTracker.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppConfig _config;

    public SecurityHeadersMiddleware(RequestDelegate next, AppConfig config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_config.Security.EnableSecurityHeaders)
        {
            // Защита от XSS и внедрения кода
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Защита от clickjacking
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Контроль кэширования для чувствительных данных
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";

            // Referrer Policy
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Content Security Policy (базовая)
            if (_config.Mode.Environment == "Production")
            {
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'self'; script-src 'self'; style-src 'self'";
            }
        }

        await _next(context);
    }
}