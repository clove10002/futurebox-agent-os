using FutureBox.Application.Interfaces;
using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;

namespace FutureBox.Infrastructure.RealTime;

public sealed class SignalRProgressReporter(
    INotificationService notifications,
    Guid projectId,
    Guid taskId,
    string agentName) : IProgressReporter
{
    public async Task ReportAsync(ProgressUpdate update, CancellationToken ct)
    {
        var hydrated = update with
        {
            ProjectId = projectId,
            TaskId = taskId,
            AgentName = agentName
        };
        await notifications.SendProgressAsync(projectId, hydrated, ct);
    }
}
