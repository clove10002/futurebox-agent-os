using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Represents a specialized worker that solves a specific type of problem within a workflow.
/// Agents never perform I/O directly — they request capabilities from Tools via IToolRegistry.
/// </summary>
public interface IAgent
{
    /// <summary>Unique name identifying this agent type.</summary>
    string Name { get; }

    /// <summary>Human-readable description of what this agent does.</summary>
    string Description { get; }

    /// <summary>Returns true if this agent can handle the given task.</summary>
    bool CanHandle(string agentName);

    /// <summary>Executes the task and returns a result.</summary>
    Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct);
}
