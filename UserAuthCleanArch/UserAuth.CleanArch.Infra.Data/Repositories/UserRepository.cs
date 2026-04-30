using UserAuth.CleanArch.Domain.Entities;
using UserAuth.CleanArch.Domain.Enums;
using UserAuth.CleanArch.Domain.Interfaces;
using UserAuth.CleanArch.Domain.Abstractions;
using UserAuth.CleanArch.Infra.Data.Context;
using MySqlConnector;
using System.Data;
using System.Text.Json;

namespace UserAuth.CleanArch.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MySqlContext _mySqlContext;

    public UserRepository(MySqlContext mySqlContext)
    {
        _mySqlContext = mySqlContext;
    }

    public async Task<User> CreateAsync(User user)
    {
        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO users (
                id, email, password_hash, phone, person_type, person_id,
                first_name, last_name, cpf, cnpj, registered_name, business_name,
                is_active, created_at, updated_at, last_activity, roles_json
            ) VALUES (
                @id, @email, @password_hash, @phone, @person_type, @person_id,
                @first_name, @last_name, @cpf, @cnpj, @registered_name, @business_name,
                @is_active, @created_at, @updated_at, @last_activity, @roles_json
            );
            """;

        AddUserParameters(command, user);
        await command.ExecuteNonQueryAsync();
        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users ORDER BY email;";

        await using var reader = await command.ExecuteReaderAsync();
        var users = new List<User>();

        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
        }

        return users;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE email = @email LIMIT 1;";
        command.Parameters.AddWithValue("@email", email.ToLowerInvariant());

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE id = @id LIMIT 1;";
        command.Parameters.AddWithValue("@id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<User?> RemoveByIdAsync(Guid id)
    {
        var user = await GetByIdAsync(id);

        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM users WHERE id = @id;";
        command.Parameters.AddWithValue("@id", id.ToString());

        await command.ExecuteNonQueryAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await using var connection = await _mySqlContext.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE users SET
                email = @email,
                password_hash = @password_hash,
                phone = @phone,
                person_type = @person_type,
                person_id = @person_id,
                first_name = @first_name,
                last_name = @last_name,
                cpf = @cpf,
                cnpj = @cnpj,
                registered_name = @registered_name,
                business_name = @business_name,
                is_active = @is_active,
                created_at = @created_at,
                updated_at = @updated_at,
                last_activity = @last_activity,
                roles_json = @roles_json
            WHERE id = @id;
            """;

        AddUserParameters(command, user);
        await command.ExecuteNonQueryAsync();
        return user;
    }

    private static void AddUserParameters(MySqlCommand command, User user)
    {
        command.Parameters.AddWithValue("@id", user.Id.ToString());
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@person_type", (int)user.PersonType);
        command.Parameters.AddWithValue("@person_id", user.GetPersonId().ToString());
        command.Parameters.AddWithValue("@first_name", user.NaturalPerson?.FirstName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@last_name", user.NaturalPerson?.LastName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@cpf", user.NaturalPerson?.CPF?.Value ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@cnpj", user.JuridicalPerson?.CNPJ.Value ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@registered_name", user.JuridicalPerson?.RegisteredName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@business_name", user.JuridicalPerson?.BusinessName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@is_active", user.IsActive);
        command.Parameters.AddWithValue("@created_at", user.CreatedAt);
        command.Parameters.AddWithValue("@updated_at", user.UpdatedAt);
        command.Parameters.AddWithValue("@last_activity", user.LastActivity ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@roles_json", JsonSerializer.Serialize(user.Roles.Select(RoleRecord.FromRole)));
    }

    private static User MapUser(IDataRecord record)
    {
        var personType = (PersonType)record.GetInt32(record.GetOrdinal("person_type"));
        var personId = Guid.Parse(GetRequiredString(record, "person_id"));

        IPerson person = personType switch
        {
            PersonType.Natural => NaturalPerson.Restore(
                personId,
                record.GetString(record.GetOrdinal("first_name")),
                record.GetString(record.GetOrdinal("last_name")),
                GetNullableString(record, "cpf")),
            PersonType.Juridical => JuridicalPerson.Restore(
                personId,
                record.GetString(record.GetOrdinal("cnpj")),
                record.GetString(record.GetOrdinal("registered_name")),
                GetNullableString(record, "business_name")),
            _ => throw new InvalidOperationException("Invalid person type stored in database.")
        };

        var rolesJson = record.GetString(record.GetOrdinal("roles_json"));
        var roles = JsonSerializer.Deserialize<List<RoleRecord>>(rolesJson) ?? new List<RoleRecord>();

        return User.Restore(
            Guid.Parse(GetRequiredString(record, "id")),
            record.GetString(record.GetOrdinal("email")),
            record.GetString(record.GetOrdinal("password_hash")),
            GetNullableString(record, "phone"),
            person,
            record.GetBoolean(record.GetOrdinal("is_active")),
            record.GetDateTime(record.GetOrdinal("created_at")),
            record.GetDateTime(record.GetOrdinal("updated_at")),
            GetNullableDateTime(record, "last_activity"),
            roles.Select(role => role.ToRole()));
    }

    private static string? GetNullableString(IDataRecord record, string name)
    {
        var ordinal = record.GetOrdinal(name);
        return record.IsDBNull(ordinal) ? null : record.GetValue(ordinal).ToString();
    }

    private static string GetRequiredString(IDataRecord record, string name)
    {
        var ordinal = record.GetOrdinal(name);
        return record.GetValue(ordinal).ToString()
            ?? throw new InvalidOperationException($"Database column '{name}' is null.");
    }

    private static DateTime? GetNullableDateTime(IDataRecord record, string name)
    {
        var ordinal = record.GetOrdinal(name);
        return record.IsDBNull(ordinal) ? null : record.GetDateTime(ordinal);
    }

    private sealed record RoleRecord(string Name, List<ClaimRecord> Claims)
    {
        public static RoleRecord FromRole(Role role)
        {
            return new RoleRecord(
                role.Name,
                role.Claims.Select(claim => new ClaimRecord(claim.Type, claim.Value)).ToList());
        }

        public Role ToRole()
        {
            return new Role(Name, Claims.Select(claim => new PermissionClaim(claim.Type, claim.Value)));
        }
    }

    private sealed record ClaimRecord(string Type, string Value);
}
