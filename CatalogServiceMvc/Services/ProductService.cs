using CatalogServiceMvc.Models;
using CatalogServiceMvc.Repositories;
using CatalogServiceMvc.Views;

namespace CatalogServiceMvc.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(ToResponse).ToArray();
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        Validate(request);

        var product = new Product
        {
            EventId = request.EventId,
            Name = request.Name.Trim(),
            Type = request.Type.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Metadata = request.Metadata
        };

        await _productRepository.CreateAsync(product);
        return ToResponse(product);
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            EventId = product.EventId,
            Name = product.Name,
            Type = product.Type,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Metadata = product.Metadata,
            IsActive = product.IsActive
        };
    }

    private static void Validate(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required");
        }

        if (request.EventId == Guid.Empty)
        {
            throw new ArgumentException("EventId is required");
        }

        if (string.IsNullOrWhiteSpace(request.Type))
        {
            throw new ArgumentException("Type is required");
        }

        if (request.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero");
        }

        if (request.StockQuantity < 0)
        {
            throw new ArgumentException("StockQuantity cannot be negative");
        }
    }
}
