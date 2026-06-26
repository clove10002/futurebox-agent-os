using FutureBox.Application.Interfaces;
using FutureBox.Domain.Interfaces;

namespace FutureBox.Infrastructure.RealTime;

public class SignalRProgressReporterFactory(INotificationService notifications) : IProgressReporterFactory
{
    public IProgressReporter Create(Guid projectId, Guid taskId, string agentName)
        => new SignalRProgressReporter(notifications, projectId, taskId, agentName);
}
