using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductionReadyApi.Infrastructure.Persistence;

namespace ProductionReadyApi.Infrastructure.Health;

public sealed class DatabaseHealthCheck(AppDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Database connection is available.")
                : HealthCheckResult.Unhealthy("Database connection is not available.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Database health check failed.",
                exception);
        }
    }
}
