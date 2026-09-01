using ProductionReadyApi.Domain.Exceptions;

namespace ProductionReadyApi.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(Guid id, string sku, string name, decimal price, int stockQuantity)
    {
        Id = id;
        SetValues(sku, name, price, stockQuantity);

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Update(string sku, string name, decimal price, int stockQuantity)
    {
        SetValues(sku, name, price, stockQuantity);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void SetValues(string sku, string name, decimal price, int stockQuantity)
    {
        var normalizedSku = sku?.Trim() ?? string.Empty;
        var normalizedName = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedSku))
        {
            throw new DomainException("SKU must not be empty.");
        }

        if (normalizedSku.Length > 64)
        {
            throw new DomainException("SKU must not exceed 64 characters.");
        }

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new DomainException("Name must not be empty.");
        }

        if (normalizedName.Length > 200)
        {
            throw new DomainException("Name must not exceed 200 characters.");
        }

        if (price < 0)
        {
            throw new DomainException("Price must be greater than or equal to 0.");
        }

        if (stockQuantity < 0)
        {
            throw new DomainException("Stock quantity must be greater than or equal to 0.");
        }

        Sku = normalizedSku.ToUpperInvariant();
        Name = normalizedName;
        Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        StockQuantity = stockQuantity;
    }
}
