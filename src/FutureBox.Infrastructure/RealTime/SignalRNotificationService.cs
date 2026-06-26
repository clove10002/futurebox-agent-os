using FutureBox.Application.Interfaces;
using FutureBox.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace FutureBox.Infrastructure.RealTime;

public class SignalRNotificationService(IHubContext<ExecutionHub, IExecutionHubClient> hub) : INotificationService
{
    public Task SendProgressAsync(Guid projectId, ProgressUpdate update, CancellationToken ct = default)
        => hub.Clients.Group(projectId.ToString()).ProgressUpdate(new
        {
            update.AgentName,
            update.Message,
            update.PercentComplete,
            update.Timestamp
        });

    public Task SendAgentCompletedAsync(Guid projectId, string agentName, CancellationToken ct = default)
        => hub.Clients.Group(projectId.ToString()).AgentCompleted(agentName);

    public Task SendAgentFailedAsync(Guid projectId, string agentName, string reason, CancellationToken ct = default)
        => hub.Clients.Group(projectId.ToString()).AgentFailed(agentName, reason);

    public Task SendWorkflowCompletedAsync(Guid projectId, CancellationToken ct = default)
        => hub.Clients.Group(projectId.ToString()).WorkflowCompleted();

    public Task SendWorkflowFailedAsync(Guid projectId, string reason, CancellationToken ct = default)
        => hub.Clients.Group(projectId.ToString()).WorkflowFailed(reason);
}
