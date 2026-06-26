using FutureBox.Application.Interfaces;
using FutureBox.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FutureBox.Application.Services;

public class ExecutionTracker(
    IProjectRepository repository,
    INotificationService notifications,
    ILogger<ExecutionTracker> logger)
{
    public async Task MarkTaskRunningAsync(AgentTask task, CancellationToken ct = default)
    {
        task.MarkRunning();
        await repository.UpdateTaskAsync(task, ct);
        logger.LogInformation("Task {Id} ({Agent}) started", task.Id, task.AgentName);
    }

    public async Task MarkTaskCompletedAsync(AgentTask task, Guid projectId, CancellationToken ct = default)
    {
        task.MarkCompleted();
        await repository.UpdateTaskAsync(task, ct);
        await notifications.SendAgentCompletedAsync(projectId, task.AgentName, ct);
        logger.LogInformation("Task {Id} ({Agent}) completed", task.Id, task.AgentName);
    }

    public async Task MarkTaskFailedAsync(AgentTask task, Guid projectId, string reason, CancellationToken ct = default)
    {
        task.MarkFailed(reason);
        await repository.UpdateTaskAsync(task, ct);
        await notifications.SendAgentFailedAsync(projectId, task.AgentName, reason, ct);
        logger.LogWarning("Task {Id} ({Agent}) failed: {Reason}", task.Id, task.AgentName, reason);
    }

    public async Task MarkTaskCancelledAsync(AgentTask task, CancellationToken ct = default)
    {
        task.MarkCancelled();
        await repository.UpdateTaskAsync(task, ct);
    }
}
