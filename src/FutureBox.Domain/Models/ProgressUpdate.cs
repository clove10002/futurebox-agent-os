namespace FutureBox.Domain.Models;

public record ProgressUpdate
{
    public required Guid ProjectId { get; init; }
    public required Guid TaskId { get; init; }
    public required string AgentName { get; init; }
    public required string Message { get; init; }
    public int PercentComplete { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
