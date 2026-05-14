using ProjectsTracker.Abstractions;
using ProjectsTracker.Enums;

namespace ProjectsTracker.Workflow;

public class TaskWorkflowService
{
    private readonly ITaskService _taskService;

    // Хранилище обработанных событий
    private readonly HashSet<string> _processedEvents = new();

    // Количество ошибок workflow
    private int _failureCount = 0;

    // Для health check
    public int FailureCount => _failureCount;

    public TaskWorkflowService(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task ProcessEvent(TaskEventRequest request)
    {
        try
        {
            // =========================
            // IDEMPOTENCY CHECK
            // =========================

            if (_processedEvents.Contains(request.IdempotencyKey))
            {
                Console.WriteLine(
                    $"[DUPLICATE] CorrelationId={request.CorrelationId}, Event={request.IdempotencyKey}");

                return Task.CompletedTask;
            }

            // =========================
            // GET TASK
            // =========================

            var task = _taskService.GetTask(
                request.ProjectId,
                request.TaskId);

            if (task == null)
            {
                throw new Exception("Task not found");
            }

            // =========================
            // STATE MACHINE
            // =========================

            switch (request.EventType)
            {
                case TaskWorkflowEvent.AssignTask:

                    if (task.Status != TaskItemStatus.Todo)
                        throw new Exception("Task must be in Todo state");

                    task.Status = TaskItemStatus.Assigned;

                    break;

                case TaskWorkflowEvent.StartTask:

                    if (task.Status != TaskItemStatus.Assigned)
                        throw new Exception("Task must be Assigned");

                    task.Status = TaskItemStatus.InProgress;

                    break;

                case TaskWorkflowEvent.CompleteTask:

                    if (task.Status != TaskItemStatus.InProgress)
                        throw new Exception("Task must be InProgress");

                    // сохраняем старое состояние для rollback
                    var previousState = task.Status;

                    try
                    {
                        task.Status = TaskItemStatus.Done;

                        // ИМИТАЦИЯ СБОЯ
                        SimulateNotificationFailure();
                    }
                    catch
                    {
                        // =========================
                        // COMPENSATION
                        // =========================

                        task.Status = previousState;

                        Console.WriteLine(
                            $"[COMPENSATION] CorrelationId={request.CorrelationId}, Rollback to {previousState}");

                        throw;
                    }

                    break;

                default:
                    throw new Exception("Unknown event");
            }

            // =========================
            // SAVE TASK
            // =========================

            _taskService.UpdateTask(request.ProjectId, task);

            // =========================
            // MARK EVENT AS PROCESSED
            // =========================

            _processedEvents.Add(request.IdempotencyKey);

            // =========================
            // LOG SUCCESS
            // =========================

            Console.WriteLine(
                $"[SUCCESS] CorrelationId={request.CorrelationId}, " +
                $"Task={task.Id}, State={task.Status}");

            return Task.CompletedTask;
        }
        catch
        {
            // увеличиваем количество ошибок
            _failureCount++;

            throw;
        }
    }
    private void SimulateNotificationFailure()
    {
        // случайный сбой для демонстрации compensation

        if (Random.Shared.Next(0, 2) == 0)
        {
            throw new Exception("Notification service failed");
        }
    }
}