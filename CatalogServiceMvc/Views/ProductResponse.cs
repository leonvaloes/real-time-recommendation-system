namespace CatalogServiceMvc.Views;

public class ProductResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Dictionary<string, object?> Metadata { get; set; } = new();
    public bool IsActive { get; set; }
}
