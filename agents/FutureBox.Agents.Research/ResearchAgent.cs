using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Research;

public class ResearchAgent : IAgent
{
    public string Name => "ResearchAgent";
    public string Description => "Researches the topic and gathers key facts, angles, and sources.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        await Report(context, "Starting topic research...", 10);
        await Task.Delay(800, ct);
        await Report(context, "Gathering key facts and sources...", 50);
        await Task.Delay(800, ct);
        await Report(context, "Research complete.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["research"] = $"[Research summary for: {context.Goal}]"
            }
        });
    }

    private Task Report(AgentContext ctx, string message, int percent) =>
        ctx.Progress.ReportAsync(new ProgressUpdate
        {
            ProjectId = ctx.ProjectId,
            TaskId = ctx.TaskId,
            AgentName = Name,
            Message = message,
            PercentComplete = percent
        }, CancellationToken.None);
}
