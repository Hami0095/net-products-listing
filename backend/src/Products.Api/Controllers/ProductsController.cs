using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.DTOs;
using Products.Api.Models;
using Products.Api.Services;

namespace Products.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Colour = request.Colour
        };

        var created = await _productService.CreateProductAsync(product);

        return CreatedAtAction(nameof(GetProducts), new { id = created.Id }, new ProductResponse(
            created.Id,
            created.Name,
            created.Description,
            created.Price,
            created.Colour,
            created.CreatedAt
        ));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery] string? colour = null)
    {
        var products = await _productService.GetAllProductsAsync(colour);
        var response = products.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Colour,
            p.CreatedAt
        ));
        return Ok(response);
    }
}
