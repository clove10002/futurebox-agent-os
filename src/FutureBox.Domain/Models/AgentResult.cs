namespace FutureBox.Domain.Models;

public record AgentResult
{
    public required string AgentName { get; init; }
    public required Guid TaskId { get; init; }
    public IReadOnlyDictionary<string, object> Outputs { get; init; } = new Dictionary<string, object>();
    public IReadOnlyList<string> GeneratedFiles { get; init; } = [];
    public DateTimeOffset CompletedAt { get; init; } = DateTimeOffset.UtcNow;
}
