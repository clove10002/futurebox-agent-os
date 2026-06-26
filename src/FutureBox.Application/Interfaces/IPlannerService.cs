using FutureBox.Domain.Entities;

namespace FutureBox.Application.Interfaces;

public interface IPlannerService
{
    ExecutionPlan CreatePlan(Guid projectId, string goal);
}
