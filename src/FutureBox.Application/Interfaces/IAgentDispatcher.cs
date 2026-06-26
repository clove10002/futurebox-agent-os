using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Application.Interfaces;

public interface IAgentDispatcher
{
    Task<Result<AgentResult>> DispatchAsync(string agentName, AgentContext context, CancellationToken ct = default);
}
