using ProjectsTracker.Config;

public class Tests
{
    public static void TestConfigurationPriority()
    {
        Console.WriteLine("=== Testing Configuration Priority ===");

        // Тест 1: Проверка загрузки из файла
        var args1 = new string[] { };
        var config1 = ConfigurationLoader.Load(args1);
        Console.WriteLine($"File config: Environment={config1.Mode.Environment}");

        // Тест 2: Проверка переопределения переменными окружения
        Environment.SetEnvironmentVariable("MODE_ENVIRONMENT", "Production");
        var config2 = ConfigurationLoader.Load(args1);
        Console.WriteLine($"After env var: Environment={config2.Mode.Environment}");

        // Тест 3: Проверка переопределения аргументами
        var args3 = new string[] { "--env", "Development" };
        var config3 = ConfigurationLoader.Load(args3);
        Console.WriteLine($"After args: Environment={config3.Mode.Environment}");

        // Тест 4: Проверка валидации некорректных данных
        try
        {
            Environment.SetEnvironmentVariable("MODE_ENVIRONMENT", "InvalidMode");
            ConfigurationLoader.Load(args1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Validation works: {ex.Message}");
        }
    }
}