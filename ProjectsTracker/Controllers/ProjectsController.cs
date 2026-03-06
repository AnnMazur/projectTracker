using Microsoft.AspNetCore.Mvc;
using ProjectsTracker.Models;
using ProjectsTracker.Abstractions;

namespace ProjectsTracker.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(_service.GetById(id));
    }

    [HttpPost]
    public IActionResult Create(Project project)
    {
        return Ok(_service.Create(project));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Project project)
    {
        project.Id = id;
        _service.Update(project);

        return Ok();
    }

    [HttpGet("{id}/tasks")]
    public IActionResult GetTasks(int id)
    {
        return Ok(_service.GetTasks(id));
    }
}
