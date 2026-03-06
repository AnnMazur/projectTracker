using ProjectsTracker.Middleware;
using ProjectsTracker.Repository;
using ProjectsTracker.Abstractions;
using ProjectsTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TimingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
