using ProjectsTracker.Enums;

namespace ProjectsTracker.Workflow;

public class TaskEventRequest
{
    public int ProjectId { get; set; }
    public int TaskId { get; set; }
    public TaskWorkflowEvent EventType { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;     // Уникальный ключ события
    public string CorrelationId { get; set; } = string.Empty;     // Сквозной идентификатор для логов
}