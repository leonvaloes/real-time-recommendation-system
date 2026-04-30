using MySqlConnector;

namespace UserAuth.CleanArch.Infra.Data.Context;

public sealed class MySqlContext
{
    private readonly string _connectionString;
    private bool _schemaInitialized;
    private readonly SemaphoreSlim _schemaLock = new(1, 1);

    public MySqlContext()
    {
        _connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
            ?? "Server=localhost;Port=3306;Database=user_auth;User=user;Password=1234;Allow User Variables=True;";
    }

    public async Task<MySqlConnection> OpenConnectionAsync()
    {
        await EnsureSchemaAsync();

        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    private async Task EnsureSchemaAsync()
    {
        if (_schemaInitialized)
        {
            return;
        }

        await _schemaLock.WaitAsync();
        try
        {
            if (_schemaInitialized)
            {
                return;
            }

            await using var connection = await OpenConnectionWithRetryAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS users (
                    id CHAR(36) NOT NULL PRIMARY KEY,
                    email VARCHAR(255) NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
                    phone VARCHAR(20) NULL,
                    person_type INT NOT NULL,
                    person_id CHAR(36) NOT NULL,
                    first_name VARCHAR(100) NULL,
                    last_name VARCHAR(100) NULL,
                    cpf VARCHAR(11) NULL,
                    cnpj VARCHAR(14) NULL,
                    registered_name VARCHAR(150) NULL,
                    business_name VARCHAR(150) NULL,
                    is_active BOOLEAN NOT NULL,
                    created_at DATETIME(6) NOT NULL,
                    updated_at DATETIME(6) NOT NULL,
                    last_activity DATETIME(6) NULL,
                    roles_json JSON NOT NULL
                );
                """;

            await command.ExecuteNonQueryAsync();
            _schemaInitialized = true;
        }
        finally
        {
            _schemaLock.Release();
        }
    }

    private async Task<MySqlConnection> OpenConnectionWithRetryAsync()
    {
        Exception? lastException = null;

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception exception)
            {
                lastException = exception;
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        throw new InvalidOperationException("Could not connect to MySQL.", lastException);
    }
}
