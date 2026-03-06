using ProjectsTracker.Models;
using ProjectsTracker.Abstractions;
using System.Collections.Generic;
using System;

namespace ProjectsTracker.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly List<Project> _projects = new();
    private int _nextProjectId = 1;

    public List<Project> GetAll() => _projects;

    public Project? GetById(int id)
    {
        return _projects.FirstOrDefault(p => p.Id == id);
    }

    public Project Create(Project project)
    {
        project.Id = _nextProjectId++;
        _projects.Add(project);
        return project;
    }

    public void Update(Project project)
    {
        var existing = GetById(project.Id);

        if (existing == null)
            throw new Exception("Project not found");

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.Status = project.Status;
    }

    public List<TaskItem> GetTasks(int projectId)
    {
        var project = GetById(projectId)
            ?? throw new Exception("Project not found");

        return project.Tasks;
    }

    public TaskItem AddTask(int projectId, TaskItem task)
    {
        var project = GetById(projectId)
            ?? throw new Exception("Project not found");

        task.Id = project.Tasks.Count == 0
            ? 1
            : project.Tasks.Max(t => t.Id) + 1;

        project.Tasks.Add(task);

        return task;
    }

    public TaskItem? GetTask(int projectId, int taskId)
    {
        var project = GetById(projectId)
            ?? throw new Exception("Project not found");

        return project.Tasks.FirstOrDefault(t => t.Id == taskId);
    }

    public void UpdateTask(int projectId, TaskItem task)
    {
        var existing = GetTask(projectId, task.Id);

        if (existing == null)
            throw new Exception("Task not found");

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.Priority = task.Priority;
    }
}