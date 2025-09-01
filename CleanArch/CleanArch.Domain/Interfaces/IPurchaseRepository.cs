namespace CleanArch.Domain.Entities
{

    public interface IPurchaseRepository
    {
        Task<IEnumerable<Purchase>> GetAllAsync();
        Task<Purchase> GetByIdAsync(int? id);
        Task<Purchase> CreateAsync(Purchase purchase);
        Task<Purchase> UpdateAsync(Purchase purchase);
        Task<Purchase> RemoveAsync(Purchase purchase);
    }
}
