using FutureBox.Shared.Results;

namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Receives a project goal and drives execution to completion.
/// Coordinates planning, agent dispatch, progress tracking, and failure recovery.
/// </summary>
public interface IOrchestrator
{
    Task<Result<Guid>> ExecuteAsync(Guid projectId, string goal, CancellationToken ct);
}
