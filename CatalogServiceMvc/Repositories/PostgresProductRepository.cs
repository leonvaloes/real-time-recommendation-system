using System.Data;
using System.Text.Json;
using CatalogServiceMvc.Data;
using CatalogServiceMvc.Models;
using Npgsql;
using NpgsqlTypes;

namespace CatalogServiceMvc.Repositories;

public class PostgresProductRepository : IProductRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly PostgresContext _context;

    public PostgresProductRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync()
    {
        const string sql = """
            SELECT id, event_id, name, type, price, stock_quantity, metadata, is_active, created_at
            FROM products
            ORDER BY created_at DESC;
            """;

        var products = new List<Product>();

        await using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(MapProduct(reader));
        }

        return products;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, event_id, name, type, price, stock_quantity, metadata, is_active, created_at
            FROM products
            WHERE id = @id;
            """;

        await using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        return await reader.ReadAsync() ? MapProduct(reader) : null;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        const string sql = """
            INSERT INTO products (
                id,
                event_id,
                name,
                type,
                price,
                stock_quantity,
                metadata,
                is_active,
                created_at
            )
            VALUES (
                @id,
                @event_id,
                @name,
                @type,
                @price,
                @stock_quantity,
                @metadata,
                @is_active,
                @created_at
            );
            """;

        await using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", product.Id);
        command.Parameters.AddWithValue("@event_id", product.EventId);
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@type", product.Type);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@stock_quantity", product.StockQuantity);
        command.Parameters.Add("@metadata", NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(product.Metadata, JsonOptions);
        command.Parameters.AddWithValue("@is_active", product.IsActive);
        command.Parameters.AddWithValue("@created_at", product.CreatedAt);

        await command.ExecuteNonQueryAsync();
        return product;
    }

    private static Product MapProduct(NpgsqlDataReader reader)
    {
        return new Product
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            EventId = reader.GetGuid(reader.GetOrdinal("event_id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Type = reader.GetString(reader.GetOrdinal("type")),
            Price = reader.GetDecimal(reader.GetOrdinal("price")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
            Metadata = DeserializeMetadata(reader.GetString(reader.GetOrdinal("metadata"))),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
        };
    }

    private static Dictionary<string, object?> DeserializeMetadata(string json)
    {
        var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, JsonOptions) ?? new();
        return values.ToDictionary(pair => pair.Key, pair => ConvertJsonElement(pair.Value));
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number when element.TryGetDecimal(out var number) => number,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.Clone()
        };
    }
}
