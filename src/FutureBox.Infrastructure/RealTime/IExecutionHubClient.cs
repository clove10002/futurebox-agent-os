namespace FutureBox.Infrastructure.RealTime;

public interface IExecutionHubClient
{
    Task ProgressUpdate(object update);
    Task AgentCompleted(string agentName);
    Task AgentFailed(string agentName, string reason);
    Task WorkflowCompleted();
    Task WorkflowFailed(string reason);
}
