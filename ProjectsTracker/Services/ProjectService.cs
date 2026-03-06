using ProjectsTracker.Models;
using ProjectsTracker.Abstractions;

namespace ProjectsTracker.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public List<Project> GetAll()
    {
        return _repository.GetAll();
    }

    public Project GetById(int id)
    {
        return _repository.GetById(id)
            ?? throw new Exception("Project not found");
    }

    public Project Create(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
            throw new Exception("Name cannot be empty");

        return _repository.Create(project);
    }

    public void Update(Project project)
    {
        _repository.Update(project);
    }

    public List<TaskItem> GetTasks(int projectId)
    {
        return _repository.GetTasks(projectId);
    }
}
