namespace Products.Api.DTOs;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Colour
);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Colour,
    DateTime CreatedAt
);
