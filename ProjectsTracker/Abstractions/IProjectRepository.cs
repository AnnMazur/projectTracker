using ProjectsTracker.Models;
using System.Collections.Generic;

namespace ProjectsTracker.Abstractions;

public interface IProjectRepository
{
    List<Project> GetAll();

    Project? GetById(int id);

    Project Create(Project project);

    void Update(Project project);

    List<TaskItem> GetTasks(int projectId);

    TaskItem AddTask(int projectId, TaskItem task);

    TaskItem? GetTask(int projectId, int taskId);

    void UpdateTask(int projectId, TaskItem task);
}