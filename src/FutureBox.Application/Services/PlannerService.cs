using FutureBox.Application.Interfaces;
using FutureBox.Domain.Entities;

namespace FutureBox.Application.Services;

public class PlannerService : IPlannerService
{
    private static readonly string[] AgentOrder =
    [
        "ResearchAgent",
        "ScriptAgent",
        "NarrationAgent",
        "SubtitleAgent",
        "AssetAgent",
        "VideoAgent",
    ];

    public ExecutionPlan CreatePlan(Guid projectId, string goal)
    {
        var tasks = AgentOrder.Select((name, i) =>
        {
            var inputs = i == 0
                ? new Dictionary<string, object> { ["topic"] = goal }
                : null;
            return AgentTask.Create(projectId, name, i + 1, inputs);
        });

        return ExecutionPlan.Create(projectId, tasks);
    }
}
