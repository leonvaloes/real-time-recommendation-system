using UserAuth.CleanArch.Domain.Validation;

namespace UserAuth.CleanArch.Domain.Entities;

public sealed class Role
{
    public string Name { get; private set; } = string.Empty;
    public List<PermissionClaim> Claims { get; private set; } = new();

    private Role()
    {
    }

    public Role(string name, IEnumerable<PermissionClaim>? claims = null)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name), "Role name is required");

        Name = name.Trim();

        if (claims is not null)
        {
            Claims.AddRange(claims);
        }
    }

    public void AddClaim(PermissionClaim claim)
    {
        var alreadyExists = Claims.Any(existingClaim =>
            existingClaim.Type == claim.Type && existingClaim.Value == claim.Value);

        if (!alreadyExists)
        {
            Claims.Add(claim);
        }
    }
}
