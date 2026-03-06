using Microsoft.AspNetCore.Mvc;
using ProjectsTracker.Models;
using ProjectsTracker.Abstractions;

namespace ProjectsTracker.Controllers;

[ApiController]
[Route("api/projects/{projectId}/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet("{taskId}")]
    public IActionResult Get(int projectId, int taskId)
    {
        return Ok(_service.GetTask(projectId, taskId));
    }

    [HttpPost]
    public IActionResult Create(int projectId, TaskItem task)
    {
        return Ok(_service.AddTask(projectId, task));
    }

    [HttpPut("{taskId}")]
    public IActionResult Update(int projectId, int taskId, TaskItem task)
    {
        task.Id = taskId;

        _service.UpdateTask(projectId, task);

        return Ok();
    }
}