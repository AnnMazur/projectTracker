using ProjectsTracker.Enums;

namespace ProjectsTracker.DTO;

public class CreateTaskRequest
{
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public TaskPriority Priority { get; set; }
}