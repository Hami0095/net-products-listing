using Products.Api.Models;

namespace Products.Api.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product);
    Task<IEnumerable<Product>> GetAllProductsAsync(string? colour = null);
}

public class ProductService : IProductService
{
    private static readonly List<Product> _products = new();

    public Task<Product> CreateProductAsync(Product product)
    {
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync(string? colour = null)
    {
        var query = _products.AsEnumerable();
        if (!string.IsNullOrEmpty(colour))
        {
            query = query.Where(p => p.Colour.Equals(colour, StringComparison.OrdinalIgnoreCase));
        }
        return Task.FromResult(query);
    }
}
