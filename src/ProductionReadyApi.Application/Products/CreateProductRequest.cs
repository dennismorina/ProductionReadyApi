namespace ProductionReadyApi.Application.Products;

public sealed record CreateProductRequest(
    string Sku,
    string Name,
    decimal Price,
    int StockQuantity);
