using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Narration;

public class NarrationAgent : IAgent
{
    public string Name => "NarrationAgent";
    public string Description => "Converts the script into spoken audio using a TTS service.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        await Report(context, "Sending script to TTS service...", 15);
        await Task.Delay(1000, ct);
        await Report(context, "Rendering audio...", 65);
        await Task.Delay(700, ct);
        await Report(context, "Narration audio ready.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["audioPath"] = "narration.mp3"
            },
            GeneratedFiles = ["narration.mp3"]
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
