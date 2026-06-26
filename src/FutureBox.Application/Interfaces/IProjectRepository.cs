using FutureBox.Domain.Entities;

namespace FutureBox.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Project> SaveAsync(Project project, CancellationToken ct = default);
    Task UpdateAsync(Project project, CancellationToken ct = default);
    Task<AgentTask> SaveTaskAsync(AgentTask task, CancellationToken ct = default);
    Task UpdateTaskAsync(AgentTask task, CancellationToken ct = default);
    Task SavePlanAsync(ExecutionPlan plan, CancellationToken ct = default);
}
