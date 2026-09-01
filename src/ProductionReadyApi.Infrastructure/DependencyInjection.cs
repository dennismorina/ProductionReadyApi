using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductionReadyApi.Application.Abstractions;
using ProductionReadyApi.Infrastructure.Health;
using ProductionReadyApi.Infrastructure.Persistence;
using ProductionReadyApi.Infrastructure.Repositories;

namespace ProductionReadyApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<DatabaseHealthCheck>();

        return services;
    }
}
