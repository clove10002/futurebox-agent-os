using FutureBox.Application.Interfaces;
using FutureBox.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FutureBox.Application.Services;

public class ProjectService(IProjectRepository repository, ILogger<ProjectService> logger)
{
    public async Task<Project> CreateAsync(string name, string goal, CancellationToken ct = default)
    {
        var project = Project.Create(name, goal);
        await repository.SaveAsync(project, ct);
        logger.LogInformation("Project {Id} created with goal: {Goal}", project.Id, goal);
        return project;
    }

    public Task<Project?> GetAsync(Guid id, CancellationToken ct = default)
        => repository.GetByIdAsync(id, ct);
}
