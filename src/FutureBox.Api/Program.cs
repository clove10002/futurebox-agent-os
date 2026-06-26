using FutureBox.Application.Extensions;
using FutureBox.Domain.Interfaces;
using FutureBox.Infrastructure.Extensions;
using FutureBox.Infrastructure.Persistence;
using FutureBox.Infrastructure.RealTime;
using FutureBox.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

// ── Agent registrations (import namespaces from each agent project)
using FutureBox.Agents.Research;
using FutureBox.Agents.Script;
using FutureBox.Agents.Narration;
using FutureBox.Agents.Subtitle;
using FutureBox.Agents.Asset;
using FutureBox.Agents.Video;

var builder = WebApplication.CreateBuilder(args);

// ── OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── SignalR
builder.Services.AddSignalR();

// ── CORS — allow React dev server in development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Agents (registered as IAgent implementations)
builder.Services.AddScoped<IAgent, ResearchAgent>();
builder.Services.AddScoped<IAgent, ScriptAgent>();
builder.Services.AddScoped<IAgent, NarrationAgent>();
builder.Services.AddScoped<IAgent, SubtitleAgent>();
builder.Services.AddScoped<IAgent, AssetAgent>();
builder.Services.AddScoped<IAgent, VideoAgent>();

// ── Build
var app = builder.Build();

// ── Ensure DB is created / migrations applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FutureBoxDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseStaticFiles();

// ── API endpoints
app.MapProjectEndpoints();

// ── SignalR hub
app.MapHub<ExecutionHub>("/hubs/execution");

// ── React SPA fallback (serves index.html for client-side routing)
app.MapFallbackToFile("index.html");

app.Run();
