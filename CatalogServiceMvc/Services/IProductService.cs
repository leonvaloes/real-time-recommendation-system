using CatalogServiceMvc.Views;

namespace CatalogServiceMvc.Services;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
}
