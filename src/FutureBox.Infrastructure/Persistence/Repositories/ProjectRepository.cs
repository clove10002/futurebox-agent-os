using FutureBox.Application.Interfaces;
using FutureBox.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutureBox.Infrastructure.Persistence.Repositories;

public class ProjectRepository(FutureBoxDbContext db) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Projects.Include("_outputs").FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Project> SaveAsync(Project project, CancellationToken ct = default)
    {
        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);
        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken ct = default)
    {
        db.Projects.Update(project);
        await db.SaveChangesAsync(ct);
    }

    public async Task<AgentTask> SaveTaskAsync(AgentTask task, CancellationToken ct = default)
    {
        db.AgentTasks.Add(task);
        await db.SaveChangesAsync(ct);
        return task;
    }

    public async Task UpdateTaskAsync(AgentTask task, CancellationToken ct = default)
    {
        db.AgentTasks.Update(task);
        await db.SaveChangesAsync(ct);
    }

    public async Task SavePlanAsync(ExecutionPlan plan, CancellationToken ct = default)
    {
        db.ExecutionPlans.Add(plan);
        foreach (var task in plan.Tasks)
            db.AgentTasks.Add(task);
        await db.SaveChangesAsync(ct);
    }
}
