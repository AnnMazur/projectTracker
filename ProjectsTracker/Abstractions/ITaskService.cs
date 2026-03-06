using ProjectsTracker.Models;

namespace ProjectsTracker.Abstractions;

public interface ITaskService
{
    TaskItem GetTask(int projectId, int taskId);

    TaskItem AddTask(int projectId, TaskItem task);

    void UpdateTask(int projectId, TaskItem task);
}