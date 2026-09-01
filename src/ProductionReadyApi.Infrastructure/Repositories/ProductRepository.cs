using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProductionReadyApi.Application.Abstractions;
using ProductionReadyApi.Application.Common;
using ProductionReadyApi.Application.Common.Exceptions;
using ProductionReadyApi.Domain.Entities;
using ProductionReadyApi.Infrastructure.Persistence;

namespace ProductionReadyApi.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(product => product.Sku == sku, cancellationToken);
    }

    public async Task<PagedResult<Product>> GetPageAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> query = dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(product =>
                product.Sku.ToLower().Contains(term) ||
                product.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(product => product.Sku)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, page, pageSize, totalCount);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        return dbContext.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public void Remove(Product product)
    {
        dbContext.Products.Remove(product);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("A product with the same unique value already exists.");
        }
    }
}
