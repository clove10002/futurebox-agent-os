using FutureBox.Application.Interfaces;
using FutureBox.Domain.Interfaces;
using FutureBox.Infrastructure.Memory;
using FutureBox.Infrastructure.Persistence;
using FutureBox.Infrastructure.Persistence.Repositories;
using FutureBox.Infrastructure.RealTime;
using FutureBox.Infrastructure.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FutureBox.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FutureBoxDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Default"),
                sql => sql.MigrationsAssembly(typeof(FutureBoxDbContext).Assembly.FullName)));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<IProgressReporterFactory, SignalRProgressReporterFactory>();

        services.AddScoped<IMemoryStore, InMemoryMemoryStore>();
        services.AddScoped<IToolRegistry, ToolRegistry>();

        return services;
    }
}
