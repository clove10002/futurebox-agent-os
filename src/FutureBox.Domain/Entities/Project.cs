using FutureBox.Shared.Enumerations;

namespace FutureBox.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Goal { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IReadOnlyList<ProjectOutput> Outputs => _outputs.AsReadOnly();

    private readonly List<ProjectOutput> _outputs = [];

    private Project() { Name = string.Empty; Goal = string.Empty; }

    public static Project Create(string name, string goal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(goal);

        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Goal = goal,
            Status = ProjectStatus.Created,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void SetStatus(ProjectStatus status)
    {
        Status = status;
        if (status is ProjectStatus.Completed or ProjectStatus.Failed or ProjectStatus.Cancelled)
            CompletedAt = DateTimeOffset.UtcNow;
    }

    public void AddOutput(ProjectOutput output) => _outputs.Add(output);
}
