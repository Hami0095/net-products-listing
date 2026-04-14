using FluentAssertions;
using Products.Api.Models;
using Products.Api.Services;

namespace Products.Tests;

public class ProductServiceTests
{
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService();
    }

    [Fact]
    public async Task CreateProductAsync_ShouldAddProductAndAssignId()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Colour = "Blue"
        };

        // Act
        var result = await _service.CreateProductAsync(product);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Product");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnFilteredProducts()
    {
        // Arrange
        await _service.CreateProductAsync(new Product { Name = "P1", Colour = "Red" });
        await _service.CreateProductAsync(new Product { Name = "P2", Colour = "Blue" });
        await _service.CreateProductAsync(new Product { Name = "P3", Colour = "Red" });

        // Act
        var redProducts = await _service.GetAllProductsAsync("Red");

        // Assert
        redProducts.Should().HaveCount(2);
        redProducts.All(p => p.Colour == "Red").Should().BeTrue();
    }
}
