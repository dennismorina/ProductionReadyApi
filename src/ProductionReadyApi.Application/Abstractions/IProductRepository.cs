using ProductionReadyApi.Application.Common;
using ProductionReadyApi.Domain.Entities;

namespace ProductionReadyApi.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken);

    Task<PagedResult<Product>> GetPageAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    void Remove(Product product);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
