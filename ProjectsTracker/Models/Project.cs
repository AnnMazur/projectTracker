using ProjectsTracker.Enums;

namespace ProjectsTracker.Models;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public List<TaskItem> Tasks { get; set; } = new();
}
