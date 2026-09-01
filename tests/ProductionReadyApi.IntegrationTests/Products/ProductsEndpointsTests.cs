using System.Net;
using System.Net.Http.Json;
using ProductionReadyApi.Application.Products;
using ProductionReadyApi.IntegrationTests.Infrastructure;

namespace ProductionReadyApi.IntegrationTests.Products;

public sealed class ProductsEndpointsTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAndGetProduct_ReturnsCreatedProduct()
    {
        var sku = $"SKU-{Guid.NewGuid():N}"[..20];
        var createRequest = new CreateProductRequest(
            sku,
            "Integration Test Product",
            42.50m,
            3);

        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(created);
        Assert.Equal(sku.ToUpperInvariant(), created.Sku);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidInput_ReturnsBadRequest()
    {
        var request = new CreateProductRequest(
            string.Empty,
            string.Empty,
            -1m,
            -1);

        var response = await _client.PostAsJsonAsync("/api/products", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ReturnsConflict()
    {
        var sku = $"SKU-{Guid.NewGuid():N}"[..20];
        var request = new CreateProductRequest(
            sku,
            "First Product",
            10m,
            1);

        var firstResponse = await _client.PostAsJsonAsync("/api/products", request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync(
            "/api/products",
            request with { Name = "Duplicate Product" }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }


    [Fact]
    public async Task GetProducts_WithSearchAndPaging_ReturnsMatchingPage()
    {
        var token = Guid.NewGuid().ToString("N")[..8];

        await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest($"A-{token}", $"Keyboard {token}", 100m, 1), TestContext.Current.CancellationToken);

        await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest($"B-{token}", $"Keyboard Pro {token}", 150m, 2), TestContext.Current.CancellationToken);

        var response = await _client.GetAsync(
            $"/api/products?search={token.ToLowerInvariant()}&page=1&pageSize=1", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ProductionReadyApi.Application.Common.PagedResult<ProductDto>>(TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
