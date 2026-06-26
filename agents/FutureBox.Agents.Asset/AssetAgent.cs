using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Asset;

public class AssetAgent : IAgent
{
    public string Name => "AssetAgent";
    public string Description => "Sources or generates visual assets — stock images, B-roll, and graphics.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        await Report(context, "Identifying required assets...", 10);
        await Task.Delay(600, ct);
        await Report(context, "Downloading visuals...", 55);
        await Task.Delay(800, ct);
        await Report(context, "Assets ready.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["assetsDir"] = "assets/"
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
