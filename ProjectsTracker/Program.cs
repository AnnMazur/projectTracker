using ProjectsTracker.Middleware;
using ProjectsTracker.Repository;
using ProjectsTracker.Abstractions;
using ProjectsTracker.Services;
using ProjectsTracker.Config;

var builder = WebApplication.CreateBuilder(args);

// Загрузка конфигурации с приоритетами
var config = ConfigurationLoader.Load(args);
builder.Services.AddSingleton(config);

// Настройка логирования в зависимости от режима
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

if (config.Mode.DetailedLogging)
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройка Swagger только для разработки
if (config.Mode.Environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Порядок middleware важен!
app.UseMiddleware<SecurityHeadersMiddleware>();      // Защитные заголовки
app.UseMiddleware<RateLimitingMiddleware>();         // Ограничение частоты
app.UseMiddleware<TrustedOriginsMiddleware>();       // Доверенные источники
app.UseMiddleware<ExceptionMiddleware>();            // Обработка ошибок
app.UseMiddleware<TimingMiddleware>();               // Замер времени
app.UseMiddleware<LoggingMiddleware>();              // Логирование

app.MapControllers();

app.Run();