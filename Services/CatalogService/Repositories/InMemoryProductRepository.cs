using CatalogService.Models;

namespace CatalogService.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public Task<IReadOnlyCollection<Product>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Product>>(_products.ToArray());
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_products.FirstOrDefault(product => product.Id == id));
    }

    public Task<Product> CreateAsync(Product product)
    {
        _products.Add(product);
        return Task.FromResult(product);
    }
}
