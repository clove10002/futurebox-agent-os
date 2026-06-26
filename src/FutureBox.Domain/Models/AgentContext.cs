using FutureBox.Domain.Interfaces;

namespace FutureBox.Domain.Models;

public record AgentContext
{
    public required Guid ProjectId { get; init; }
    public required Guid TaskId { get; init; }
    public required string Goal { get; init; }
    public required IReadOnlyDictionary<string, object> Inputs { get; init; }
    public required IToolRegistry Tools { get; init; }
    public required IProgressReporter Progress { get; init; }
    public required IMemoryStore Memory { get; init; }
}
