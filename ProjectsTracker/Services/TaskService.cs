using ProjectsTracker.Models;
using ProjectsTracker.Abstractions;
using System;

namespace ProjectsTracker.Services;

public class TaskService : ITaskService
{
    private readonly IProjectRepository _repository;

    public TaskService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public TaskItem GetTask(int projectId, int taskId)
    {
        return _repository.GetTask(projectId, taskId)
            ?? throw new Exception("Task not found");
    }

    public TaskItem AddTask(int projectId, TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new Exception("Task title cannot be empty");

        return _repository.AddTask(projectId, task);
    }

    public void UpdateTask(int projectId, TaskItem task)
    {
        _repository.UpdateTask(projectId, task);
    }
}