using FutureBox.Shared.Enumerations;

namespace FutureBox.Domain.Entities;

public class AgentTask
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string AgentName { get; private set; }
    public int Order { get; private set; }
    public AgentTaskStatus Status { get; private set; }
    public IReadOnlyDictionary<string, object> Inputs => _inputs;
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private readonly Dictionary<string, object> _inputs = [];

    private AgentTask() { AgentName = string.Empty; }

    public static AgentTask Create(Guid projectId, string agentName, int order, Dictionary<string, object>? inputs = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(agentName);

        var task = new AgentTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            AgentName = agentName,
            Order = order,
            Status = AgentTaskStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        if (inputs is not null)
            foreach (var kv in inputs)
                task._inputs[kv.Key] = kv.Value;

        return task;
    }

    public void MarkRunning()
    {
        Status = AgentTaskStatus.Running;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCompleted()
    {
        Status = AgentTaskStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        Status = AgentTaskStatus.Failed;
        FailureReason = reason;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCancelled()
    {
        Status = AgentTaskStatus.Cancelled;
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
