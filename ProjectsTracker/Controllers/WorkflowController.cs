using Microsoft.AspNetCore.Mvc;
using ProjectsTracker.Workflow;

namespace ProjectsTracker.Controllers;

[ApiController]
[Route("api/workflow")]
public class WorkflowController : ControllerBase
{
    private readonly TaskWorkflowService _workflowService;

    public WorkflowController(TaskWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpPost("event")]
    public async Task<IActionResult> ProcessEvent(
        [FromBody] TaskEventRequest request)
    {
        await _workflowService.ProcessEvent(request);

        return Ok(new
        {
            Message = "Event processed successfully",
            CorrelationId = request.CorrelationId
        });
    }
}