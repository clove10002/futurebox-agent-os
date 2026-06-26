using FutureBox.Domain.Models;

namespace FutureBox.Application.Interfaces;

public interface INotificationService
{
    Task SendProgressAsync(Guid projectId, ProgressUpdate update, CancellationToken ct = default);
    Task SendAgentCompletedAsync(Guid projectId, string agentName, CancellationToken ct = default);
    Task SendAgentFailedAsync(Guid projectId, string agentName, string reason, CancellationToken ct = default);
    Task SendWorkflowCompletedAsync(Guid projectId, CancellationToken ct = default);
    Task SendWorkflowFailedAsync(Guid projectId, string reason, CancellationToken ct = default);
}
