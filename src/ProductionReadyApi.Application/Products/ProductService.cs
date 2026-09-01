using ProductionReadyApi.Application.Abstractions;
using ProductionReadyApi.Application.Common;
using ProductionReadyApi.Application.Common.Exceptions;
using ProductionReadyApi.Domain.Entities;

namespace ProductionReadyApi.Application.Products;

public sealed class ProductService(IProductRepository repository)
{
    public async Task<PagedResult<ProductDto>> GetPageAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ProductInputValidator.ValidatePaging(page, pageSize);

        var result = await repository.GetPageAsync(search, page, pageSize, cancellationToken);

        return new PagedResult<ProductDto>(
            result.Items.Select(Map).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' was not found.");

        return Map(product);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ProductInputValidator.Validate(request.Sku, request.Name, request.Price, request.StockQuantity);

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        var existing = await repository.GetBySkuAsync(normalizedSku, cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException($"A product with SKU '{normalizedSku}' already exists.");
        }

        var product = new Product(
            Guid.NewGuid(),
            normalizedSku,
            request.Name,
            request.Price,
            request.StockQuantity);

        await repository.AddAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        ProductInputValidator.Validate(request.Sku, request.Name, request.Price, request.StockQuantity);

        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' was not found.");

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();

        if (!string.Equals(product.Sku, normalizedSku, StringComparison.Ordinal))
        {
            var existing = await repository.GetBySkuAsync(normalizedSku, cancellationToken);
            if (existing is not null && existing.Id != id)
            {
                throw new ConflictException($"A product with SKU '{normalizedSku}' already exists.");
            }
        }

        product.Update(normalizedSku, request.Name, request.Price, request.StockQuantity);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' was not found.");

        repository.Remove(product);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private static ProductDto Map(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Price,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
