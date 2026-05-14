using ProjectsTracker.Middleware;
using ProjectsTracker.Repository;
using ProjectsTracker.Abstractions;
using ProjectsTracker.Services;
using ProjectsTracker.Config;
using ProjectsTracker.Workflow;
using ProjectsTracker.Health;
using ProjectsTracker.Metrics;

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

builder.Services.AddSingleton<TaskMetrics>();
builder.Services.AddScoped<TaskWorkflowService>();

builder.Services.AddHealthChecks()
    .AddCheck<WorkflowHealthCheck>("workflow");

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

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();      // Защитные заголовки
app.UseMiddleware<RateLimitingMiddleware>();         // Ограничение частоты
app.UseMiddleware<TrustedOriginsMiddleware>();       // Доверенные источники
app.UseMiddleware<ExceptionMiddleware>();            // Обработка ошибок
app.UseMiddleware<TimingMiddleware>();               // Замер времени
app.UseMiddleware<LoggingMiddleware>();              // Логирование

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapControllers();

app.Run();