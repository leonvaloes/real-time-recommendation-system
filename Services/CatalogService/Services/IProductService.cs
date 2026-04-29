using CatalogService.Views;

namespace CatalogService.Services;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
}
