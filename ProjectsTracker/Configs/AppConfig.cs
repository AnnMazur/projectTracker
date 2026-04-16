namespace ProjectsTracker.Config;

public class AppConfig
{
    public SecurityConfig Security { get; set; } = new();
    public RateLimitConfig RateLimit { get; set; } = new();
    public ModeConfig Mode { get; set; } = new();
    public LoggingConfig Logging { get; set; } = new();
}

public class SecurityConfig
{
    public List<string> TrustedOrigins { get; set; } = new();
    public bool EnableCors { get; set; } = true;
    public bool EnableSecurityHeaders { get; set; } = true;
}

public class RateLimitConfig
{
    public int GeneralPermitLimit { get; set; } = 100;
    public int GeneralWindowSeconds { get; set; } = 60;
    public int CreatePermitLimit { get; set; } = 10;
    public int CreateWindowSeconds { get; set; } = 60;
    public bool EnableRateLimiting { get; set; } = true;
}

public class ModeConfig
{
    public string Environment { get; set; } = "Development"; // Development / Production
    public bool VerboseErrors { get; set; } = true;
    public bool DetailedLogging { get; set; } = true;
}

public class LoggingConfig
{
    public string Level { get; set; } = "Information";
    public bool LogRateLimitHits { get; set; } = true;
}
