using FutureBox.Application.Services;
using FutureBox.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureBox.Api.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapPost("/", CreateProject)
             .WithName("CreateProject")
             .WithSummary("Create a new project and start execution");

        group.MapGet("/{id:guid}", GetProject)
             .WithName("GetProject")
             .WithSummary("Get project status and details");

        return app;
    }

    private static async Task<IResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        ProjectService projectService,
        IOrchestrator orchestrator,
        IServiceScopeFactory scopeFactory,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Goal))
            return Results.BadRequest(new { error = "Goal is required." });

        var name = request.Name ?? $"Project {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}";
        var project = await projectService.CreateAsync(name, request.Goal, ct);

        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            var orch = scope.ServiceProvider.GetRequiredService<IOrchestrator>();
            await orch.ExecuteAsync(project.Id, request.Goal, CancellationToken.None);
        });

        return Results.Ok(new { projectId = project.Id, name = project.Name });
    }

    private static async Task<IResult> GetProject(
        Guid id,
        ProjectService projectService,
        CancellationToken ct)
    {
        var project = await projectService.GetAsync(id, ct);
        if (project is null)
            return Results.NotFound(new { error = $"Project {id} not found." });

        return Results.Ok(new
        {
            project.Id,
            project.Name,
            project.Goal,
            Status = project.Status.ToString(),
            project.CreatedAt,
            project.CompletedAt,
            Outputs = project.Outputs.Select(o => new
            {
                o.FileName,
                o.MimeType,
                o.FileSizeBytes
            })
        });
    }
}

public record CreateProjectRequest(string Goal, string? Name = null);
