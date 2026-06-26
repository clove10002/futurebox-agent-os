namespace FutureBox.Domain.Entities;

public class ExecutionPlan
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public IReadOnlyList<AgentTask> Tasks => _tasks.AsReadOnly();

    private readonly List<AgentTask> _tasks = [];

    private ExecutionPlan() { }

    public static ExecutionPlan Create(Guid projectId, IEnumerable<AgentTask> tasks)
    {
        var plan = new ExecutionPlan
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId
        };
        plan._tasks.AddRange(tasks);
        return plan;
    }
}
