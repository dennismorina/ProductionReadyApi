namespace ProductionReadyApi.Application.Products;

public sealed record UpdateProductRequest(
    string Sku,
    string Name,
    decimal Price,
    int StockQuantity);
