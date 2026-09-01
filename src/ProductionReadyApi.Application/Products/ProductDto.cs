namespace ProductionReadyApi.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    decimal Price,
    int StockQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
