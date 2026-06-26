using FutureBox.Domain.Interfaces;

namespace FutureBox.Application.Interfaces;

public interface IProgressReporterFactory
{
    IProgressReporter Create(Guid projectId, Guid taskId, string agentName);
}
