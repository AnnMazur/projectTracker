// Middleware/RateLimitingMiddleware.cs - исправленная версия
using ProjectsTracker.Config;
using System.Collections.Concurrent;

namespace ProjectsTracker.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly AppConfig _config;

    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, AppConfig config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_config.RateLimit.EnableRateLimiting)
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var isCreateRequest = IsCreateEndpoint(context.Request);

        var limit = isCreateRequest
            ? _config.RateLimit.CreatePermitLimit
            : _config.RateLimit.GeneralPermitLimit;

        var windowSeconds = isCreateRequest
            ? _config.RateLimit.CreateWindowSeconds
            : _config.RateLimit.GeneralWindowSeconds;

        var clientInfo = _clients.GetOrAdd(clientId, new ClientRequestInfo());

        lock (clientInfo)
        {
            var now = DateTime.UtcNow;
            clientInfo.Requests.RemoveAll(r => r < now.AddSeconds(-windowSeconds));

            if (clientInfo.Requests.Count >= limit)
            {
                var retryAfter = windowSeconds - (now - clientInfo.Requests.First()).Seconds;

                if (_config.Logging.LogRateLimitHits)
                {
                    _logger.LogWarning("Rate limit exceeded for {ClientId} on {Endpoint}. Limit: {Limit} per {Window}s",
                        clientId, context.Request.Path, limit, windowSeconds);
                }

                context.Response.StatusCode = 429;
                context.Response.Headers.RetryAfter = retryAfter.ToString();

                // Исправление: используем object и создаем разные объекты
                object response;
                if (_config.Mode.VerboseErrors)
                {
                    response = new RateLimitErrorResponse
                    {
                        Error = "Too Many Requests",
                        Message = $"Rate limit exceeded. Try again in {retryAfter} seconds",
                        Limit = limit,
                        WindowSeconds = windowSeconds
                    };
                }
                else
                {
                    response = new RateLimitErrorResponse
                    {
                        Error = "Too Many Requests"
                    };
                }

                context.Response.WriteAsJsonAsync(response).Wait();
                return;
            }

            clientInfo.Requests.Add(now);
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Используем IP-адрес как идентификатор клиента
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return ip;
    }

    private bool IsCreateEndpoint(HttpRequest request)
    {
        // POST запросы на создание имеют более строгие лимиты
        return request.Method == "POST" &&
               (request.Path.ToString().Contains("/tasks") ||
                request.Path.ToString() == "/api/projects");
    }

    private class ClientRequestInfo
    {
        public List<DateTime> Requests { get; set; } = new();
    }
}

// DTO для ответа rate limiting
public class RateLimitErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Message { get; set; }
    public int? Limit { get; set; }
    public int? WindowSeconds { get; set; }
}