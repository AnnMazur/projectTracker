// Config/ConfigurationLoader.cs - исправленная версия
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ProjectsTracker.Config;

public class ConfigurationLoader
{
    private const string ConfigFilePath = "appsettings.json";

    public static AppConfig Load(string[] args)
    {
        var config = new AppConfig();

        // 1. Загрузка из файла (наименьший приоритет)
        if (File.Exists(ConfigFilePath))
        {
            var json = File.ReadAllText(ConfigFilePath);
            var fileConfig = JsonSerializer.Deserialize<AppConfig>(json);
            if (fileConfig != null)
                config = fileConfig;
        }

        // 2. Переопределение из переменных окружения (средний приоритет)
        LoadFromEnvironmentVariables(config);

        // 3. Переопределение из аргументов командной строки (наивысший приоритет)
        LoadFromCommandLineArgs(config, args);

        // Валидация конфигурации
        ValidateConfiguration(config);

        return config;
    }

    private static void LoadFromEnvironmentVariables(AppConfig config)
    {
        // Security:TRUSTED_ORIGINS=http://localhost:3000,https://example.com
        var trustedOrigins = Environment.GetEnvironmentVariable("SECURITY_TRUSTED_ORIGINS");
        if (!string.IsNullOrEmpty(trustedOrigins))
        {
            config.Security.TrustedOrigins = trustedOrigins.Split(',').ToList();
        }

        // MODE_ENVIRONMENT=Production
        var env = Environment.GetEnvironmentVariable("MODE_ENVIRONMENT");
        if (!string.IsNullOrEmpty(env))
            config.Mode.Environment = env;

        // RATE_LIMIT_GENERAL_PERMIT_LIMIT=50
        var generalLimit = Environment.GetEnvironmentVariable("RATE_LIMIT_GENERAL_PERMIT_LIMIT");
        if (int.TryParse(generalLimit, out var gl))
            config.RateLimit.GeneralPermitLimit = gl;

        var createLimit = Environment.GetEnvironmentVariable("RATE_LIMIT_CREATE_PERMIT_LIMIT");
        if (int.TryParse(createLimit, out var cl))
            config.RateLimit.CreatePermitLimit = cl;
    }

    private static void LoadFromCommandLineArgs(AppConfig config, string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--env":
                case "-e":
                    if (i + 1 < args.Length)
                        config.Mode.Environment = args[++i];
                    break;
                case "--trusted-origins":
                case "-to":
                    if (i + 1 < args.Length)
                        config.Security.TrustedOrigins = args[++i].Split(',').ToList();
                    break;
                case "--verbose-errors":
                case "-ve":
                    if (i + 1 < args.Length)
                    {
                        // Исправление: используем временную переменную
                        if (bool.TryParse(args[i + 1], out var verboseErrors))
                        {
                            config.Mode.VerboseErrors = verboseErrors;
                            i++;
                        }
                    }
                    break;
                case "--detailed-logging":
                case "-dl":
                    if (i + 1 < args.Length)
                    {
                        if (bool.TryParse(args[i + 1], out var detailedLogging))
                        {
                            config.Mode.DetailedLogging = detailedLogging;
                            i++;
                        }
                    }
                    break;
                case "--general-limit":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out var generalLimit))
                    {
                        config.RateLimit.GeneralPermitLimit = generalLimit;
                        i++;
                    }
                    break;
                case "--create-limit":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out var createLimit))
                    {
                        config.RateLimit.CreatePermitLimit = createLimit;
                        i++;
                    }
                    break;
            }
        }
    }

    private static void ValidateConfiguration(AppConfig config)
    {
        var errors = new List<string>();

        // Проверка режима работы
        if (config.Mode.Environment != "Development" && config.Mode.Environment != "Production")
            errors.Add($"Invalid environment: {config.Mode.Environment}. Must be 'Development' or 'Production'");

        // Проверка доверенных источников на корректность URL
        foreach (var origin in config.Security.TrustedOrigins)
        {
            if (!IsValidUrl(origin))
                errors.Add($"Invalid trusted origin URL: {origin}");
        }

        // Проверка лимитов
        if (config.RateLimit.GeneralPermitLimit <= 0)
            errors.Add("General permit limit must be positive");

        if (config.RateLimit.CreatePermitLimit <= 0)
            errors.Add("Create permit limit must be positive");

        if (errors.Count > 0)
        {
            Console.WriteLine("Configuration validation failed:");
            foreach (var error in errors)
                Console.WriteLine($"  - {error}");
            throw new InvalidOperationException("Invalid configuration. Application cannot start.");
        }

        Console.WriteLine($"Configuration loaded: Environment={config.Mode.Environment}, " +
                         $"TrustedOrigins={config.Security.TrustedOrigins.Count}, " +
                         $"RateLimiting={config.RateLimit.EnableRateLimiting}");
    }

    private static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Базовая проверка формата URL
        var regex = new Regex(@"^https?:\/\/(localhost|[\w\-]+(\.[\w\-]+)+)(:\d+)?$");
        return regex.IsMatch(url);
    }
}