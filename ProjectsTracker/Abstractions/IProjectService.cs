using ProjectsTracker.Models;
using System.Collections.Generic;

namespace ProjectsTracker.Abstractions;

public interface IProjectService
{
	List<Project> GetAll();

	Project GetById(int id);

	Project Create(Project project);

	void Update(Project project);

	List<TaskItem> GetTasks(int projectId);
}