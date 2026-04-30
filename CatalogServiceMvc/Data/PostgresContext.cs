using Npgsql;

namespace CatalogServiceMvc.Data;

public class PostgresContext
{
    private const int MaxAttempts = 12;
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

    public PostgresContext()
    {
        ConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=catalog;Username=catalog;Password=1234";

        EnsureDatabaseAsync().GetAwaiter().GetResult();
    }

    public string ConnectionString { get; }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }

    private async Task EnsureDatabaseAsync()
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync();

                const string sql = """
                    CREATE TABLE IF NOT EXISTS products (
                        id UUID PRIMARY KEY,
                        event_id UUID NOT NULL,
                        name VARCHAR(120) NOT NULL,
                        type VARCHAR(50) NOT NULL,
                        price NUMERIC(10, 2) NOT NULL,
                        stock_quantity INTEGER NOT NULL,
                        metadata JSONB NOT NULL DEFAULT '{}'::jsonb,
                        is_active BOOLEAN NOT NULL,
                        created_at TIMESTAMPTZ NOT NULL
                    );

                    CREATE INDEX IF NOT EXISTS idx_products_event_id ON products(event_id);
                    CREATE INDEX IF NOT EXISTS idx_products_type ON products(type);
                    CREATE INDEX IF NOT EXISTS idx_products_metadata ON products USING GIN(metadata);
                    """;

                await using var command = new NpgsqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                return;
            }
            catch when (attempt < MaxAttempts)
            {
                await Task.Delay(Delay);
            }
        }
    }
}
