using ProductionReadyApi.Domain.Entities;
using ProductionReadyApi.Domain.Exceptions;

namespace ProductionReadyApi.UnitTests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Constructor_NormalizesSkuAndPrice()
    {
        var product = new Product(
            Guid.NewGuid(),
            " kb-001 ",
            "Mechanical Keyboard",
            129.995m,
            10);

        Assert.Equal("KB-001", product.Sku);
        Assert.Equal(130.00m, product.Price);
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsDomainException()
    {
        var action = () => new Product(
            Guid.NewGuid(),
            "KB-001",
            "Mechanical Keyboard",
            -1m,
            10);

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Price must be greater than or equal to 0.", exception.Message);
    }

    [Fact]
    public void Update_ChangesValuesAndUpdatedAt()
    {
        var product = new Product(
            Guid.NewGuid(),
            "KB-001",
            "Mechanical Keyboard",
            129.90m,
            10);

        var previousUpdatedAt = product.UpdatedAt;

        product.Update(
            "kb-002",
            "Mechanical Keyboard Pro",
            149.90m,
            5);

        Assert.Equal("KB-002", product.Sku);
        Assert.Equal("Mechanical Keyboard Pro", product.Name);
        Assert.Equal(149.90m, product.Price);
        Assert.Equal(5, product.StockQuantity);
        Assert.True(product.UpdatedAt >= previousUpdatedAt);
    }
}
