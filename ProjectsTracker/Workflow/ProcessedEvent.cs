namespace ProjectsTracker.Workflow;

public class ProcessedEvent
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}