using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Script;

public class ScriptAgent : IAgent
{
    public string Name => "ScriptAgent";
    public string Description => "Writes the video script from the research output.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        var research = await context.Memory.GetAsync<string>("ResearchAgent.research", ct) ?? context.Goal;

        await Report(context, "Drafting script outline...", 20);
        await Task.Delay(700, ct);
        await Report(context, "Writing narration copy...", 60);
        await Task.Delay(700, ct);
        await Report(context, "Script finalised.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["script"] = $"[Script based on: {research}]"
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
