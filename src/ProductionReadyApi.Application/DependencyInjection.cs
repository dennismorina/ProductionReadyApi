using Microsoft.Extensions.DependencyInjection;
using ProductionReadyApi.Application.Products;

namespace ProductionReadyApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        return services;
    }
}
