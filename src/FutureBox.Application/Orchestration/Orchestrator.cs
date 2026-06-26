using FutureBox.Application.Interfaces;
using FutureBox.Application.Services;
using FutureBox.Domain.Entities;
using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Enumerations;
using FutureBox.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FutureBox.Application.Orchestration;

public class Orchestrator(
    IProjectRepository repository,
    IPlannerService planner,
    IAgentDispatcher dispatcher,
    INotificationService notifications,
    IProgressReporterFactory progressFactory,
    IMemoryStore memory,
    IToolRegistry tools,
    ExecutionTracker tracker,
    ILogger<Orchestrator> logger) : IOrchestrator
{
    public async Task<Result<Guid>> ExecuteAsync(Guid projectId, string goal, CancellationToken ct)
    {
        var project = await repository.GetByIdAsync(projectId, ct);
        if (project is null)
            return Result.Failure<Guid>(Error.NotFound($"Project {projectId} not found"));

        project.SetStatus(ProjectStatus.Planning);
        await repository.UpdateAsync(project, ct);

        var plan = planner.CreatePlan(projectId, goal);
        await repository.SavePlanAsync(plan, ct);

        project.SetStatus(ProjectStatus.Running);
        await repository.UpdateAsync(project, ct);

        foreach (var task in plan.Tasks.OrderBy(t => t.Order))
        {
            if (ct.IsCancellationRequested)
            {
                await tracker.MarkTaskCancelledAsync(task, CancellationToken.None);
                continue;
            }

            await tracker.MarkTaskRunningAsync(task, ct);

            var progress = progressFactory.Create(projectId, task.Id, task.AgentName);
            var context = new AgentContext
            {
                ProjectId = projectId,
                TaskId = task.Id,
                Goal = goal,
                Inputs = task.Inputs,
                Tools = tools,
                Progress = progress,
                Memory = memory
            };

            var result = await dispatcher.DispatchAsync(task.AgentName, context, ct);

            if (result.IsSuccess)
            {
                await tracker.MarkTaskCompletedAsync(task, projectId, ct);

                foreach (var (key, value) in result.Value!.Outputs)
                    await memory.SetAsync($"{task.AgentName}.{key}", value, CancellationToken.None);
            }
            else
            {
                var error = result.Error!;
                await tracker.MarkTaskFailedAsync(task, projectId, error.Message, CancellationToken.None);
                project.SetStatus(ProjectStatus.Failed);
                await repository.UpdateAsync(project, CancellationToken.None);
                await notifications.SendWorkflowFailedAsync(projectId, error.Message, CancellationToken.None);
                logger.LogError("Workflow {ProjectId} failed at {Agent}: {Error}", projectId, task.AgentName, error.Message);
                return Result.Failure<Guid>(error);
            }
        }

        project.SetStatus(ProjectStatus.Completed);
        await repository.UpdateAsync(project, CancellationToken.None);
        await notifications.SendWorkflowCompletedAsync(projectId, CancellationToken.None);
        logger.LogInformation("Project {ProjectId} completed successfully", projectId);
        return Result.Success(projectId);
    }
}
