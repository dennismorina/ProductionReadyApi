using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProductionReadyApi.Api.ErrorHandling;
using ProductionReadyApi.Application;
using ProductionReadyApi.Infrastructure;
using ProductionReadyApi.Infrastructure.Health;
using ProductionReadyApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

app.MapHealthChecks("/health");

await app.Services.InitializeDatabaseAsync(app.Configuration);
await app.RunAsync();

public partial class Program
{
}
