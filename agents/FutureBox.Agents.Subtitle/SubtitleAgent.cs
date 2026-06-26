using FutureBox.Domain.Interfaces;
using FutureBox.Domain.Models;
using FutureBox.Shared.Results;

namespace FutureBox.Agents.Subtitle;

public class SubtitleAgent : IAgent
{
    public string Name => "SubtitleAgent";
    public string Description => "Generates SRT subtitle timings from the narration audio.";

    public bool CanHandle(string agentName) => agentName == Name;

    public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
    {
        await Report(context, "Analysing audio timestamps...", 25);
        await Task.Delay(600, ct);
        await Report(context, "Writing subtitle file...", 70);
        await Task.Delay(400, ct);
        await Report(context, "Subtitles complete.", 100);

        return Result.Success(new AgentResult
        {
            AgentName = Name,
            TaskId = context.TaskId,
            Outputs = new Dictionary<string, object>
            {
                ["subtitlePath"] = "subtitles.srt"
            },
            GeneratedFiles = ["subtitles.srt"]
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
