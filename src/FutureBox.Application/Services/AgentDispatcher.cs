using FutureBox.Application.Interfaces;
using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FutureBox.Application.Services;

public class AgentDispatcher(IEnumerable<IAgent> agents, ILogger<AgentDispatcher> logger) : IAgentDispatcher
{
    public async Task<Result<AgentResult>> DispatchAsync(string agentName, AgentContext context, CancellationToken ct = default)
    {
        var agent = agents.FirstOrDefault(a => a.CanHandle(agentName));
        if (agent is null)
        {
            logger.LogWarning("No agent registered for name: {AgentName}", agentName);
            return Result.Failure<AgentResult>(Error.NotFound($"No agent registered for '{agentName}'"));
        }

        logger.LogInformation("Dispatching to {AgentName}", agentName);
        return await agent.ExecuteAsync(context, ct);
    }
}
