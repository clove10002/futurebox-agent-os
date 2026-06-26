using Microsoft.AspNetCore.SignalR;

namespace FutureBox.Infrastructure.RealTime;

public class ExecutionHub : Hub<IExecutionHubClient>
{
    public async Task JoinProject(string projectId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, projectId);

    public async Task LeaveProject(string projectId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
}
