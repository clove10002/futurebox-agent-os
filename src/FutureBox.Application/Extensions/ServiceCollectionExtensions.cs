using FutureBox.Application.Interfaces;
using FutureBox.Application.Orchestration;
using FutureBox.Application.Services;
using FutureBox.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FutureBox.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProjectService>();
        services.AddScoped<IPlannerService, PlannerService>();
        services.AddScoped<IAgentDispatcher, AgentDispatcher>();
        services.AddScoped<ExecutionTracker>();
        services.AddScoped<IOrchestrator, Orchestrator>();
        return services;
    }
}
