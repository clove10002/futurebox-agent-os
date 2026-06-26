using FutureBox.Domain.Entities;
using FutureBox.Shared.Enumerations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FutureBox.Infrastructure.Persistence;

public class FutureBoxDbContext(DbContextOptions<FutureBoxDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<AgentTask> AgentTasks => Set<AgentTask>();
    public DbSet<ProjectOutput> ProjectOutputs => Set<ProjectOutput>();
    public DbSet<ExecutionPlan> ExecutionPlans => Set<ExecutionPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Goal).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Status).HasConversion<int>();
            entity.Property(p => p.CreatedAt);
            entity.Property(p => p.CompletedAt);

            entity.HasMany(p => p.Outputs)
                  .WithOne()
                  .HasForeignKey("ProjectId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(p => p.Outputs)
                  .HasField("_outputs")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<AgentTask>(entity =>
        {
            entity.ToTable("AgentTasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ProjectId);
            entity.Property(t => t.AgentName).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Order);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.FailureReason).HasMaxLength(1000);
            entity.Property(t => t.CreatedAt);
            entity.Property(t => t.StartedAt);
            entity.Property(t => t.CompletedAt);

            entity.Property<string>("InputsJson")
                  .HasColumnName("Inputs")
                  .HasColumnType("nvarchar(max)")
                  .HasDefaultValue("{}");

            entity.Ignore(t => t.Inputs);
        });

        modelBuilder.Entity<ProjectOutput>(entity =>
        {
            entity.ToTable("ProjectOutputs");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.ProjectId);
            entity.Property(o => o.FileName).IsRequired().HasMaxLength(260);
            entity.Property(o => o.FilePath).IsRequired().HasMaxLength(1000);
            entity.Property(o => o.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(o => o.FileSizeBytes);
            entity.Property(o => o.CreatedAt);
        });

        modelBuilder.Entity<ExecutionPlan>(entity =>
        {
            entity.ToTable("ExecutionPlans");
            entity.HasKey(ep => ep.Id);
            entity.Property(ep => ep.ProjectId);
            entity.Ignore(ep => ep.Tasks);
        });
    }
}
