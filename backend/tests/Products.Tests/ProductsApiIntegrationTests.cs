using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Api.DTOs;

namespace Products.Tests;

public class ProductsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/products", new CreateProductRequest("Name", "Desc", 10, "Red"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAndListProducts_WithAuth_ReturnsSuccess()
    {
        // 1. Login to get token
        var loginResponse = await _client.PostAsync("/api/auth/login", null);
        loginResponse.EnsureSuccessStatusCode();
        var authData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var token = authData!.Token;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create a product
        var createResponse = await _client.PostAsJsonAsync("/api/products", new CreateProductRequest("Integration Test Product", "Desc", 99.99m, "Green"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // 3. List products
        var listResponse = await _client.GetAsync("/api/products?colour=Green");
        listResponse.EnsureSuccessStatusCode();
        var products = await listResponse.Content.ReadFromJsonAsync<IEnumerable<ProductResponse>>();

        products.Should().NotBeEmpty();
        products!.First().Name.Should().Be("Integration Test Product");
        products!.First().Colour.Should().Be("Green");
    }

    private record LoginResponse(string Token);
}
