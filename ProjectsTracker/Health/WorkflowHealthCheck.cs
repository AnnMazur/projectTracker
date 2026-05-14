using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProjectsTracker.Workflow;

namespace ProjectsTracker.Health;

public class WorkflowHealthCheck : IHealthCheck
{
    private readonly TaskWorkflowService _workflowService;

    public WorkflowHealthCheck(TaskWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_workflowService.FailureCount > 5)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Too many workflow failures"));
        }

        return Task.FromResult(
            HealthCheckResult.Healthy(
                "Workflow service is healthy"));
    }
}