using FutureBox.Domain.Models;

namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Allows agents to report incremental progress during execution.
/// Progress flows through SignalR to the UI — never use Console or ILogger for user-facing progress.
/// </summary>
public interface IProgressReporter
{
    Task ReportAsync(ProgressUpdate update, CancellationToken ct);
}
