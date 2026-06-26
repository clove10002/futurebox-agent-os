using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Video;

public class VideoAgent : IAgent
{
    public string Name => "VideoAgent";
    public string Description => "Assembles all assets into the final video using FFmpeg.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        await Report(context, "Assembling timeline...", 10);
        await Task.Delay(600, ct);
        await Report(context, "Rendering video...", 40);
        await Task.Delay(1200, ct);
        await Report(context, "Encoding output...", 80);
        await Task.Delay(600, ct);
        await Report(context, "video.mp4 ready.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["videoPath"] = "video.mp4"
            },
            GeneratedFiles = ["video.mp4"]
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
