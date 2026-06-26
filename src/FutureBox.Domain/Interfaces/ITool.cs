using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Represents a capability that performs a single type of action.
/// Tools contain no business logic and are stateless where possible.
/// </summary>
public interface ITool
{
    /// <summary>Unique name identifying this tool type.</summary>
    string Name { get; }

    /// <summary>Human-readable description of what this tool does.</summary>
    string Description { get; }

    /// <summary>Executes the tool's action with the given request.</summary>
    Task<Result<ToolResult>> ExecuteAsync(ToolRequest request, CancellationToken ct);
}
