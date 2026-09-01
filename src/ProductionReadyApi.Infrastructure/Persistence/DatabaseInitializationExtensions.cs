using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductionReadyApi.Infrastructure.Persistence;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var applyMigrations = configuration.GetValue("Database:ApplyMigrations", true);
        var ensureCreated = configuration.GetValue("Database:EnsureCreated", false);

        if (applyMigrations)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            return;
        }

        if (ensureCreated)
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
