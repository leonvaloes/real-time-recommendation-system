namespace CatalogServiceMvc.Views;

public class CreateProductRequest
{
    public Guid EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
