using CleanArch.Domain.Validation;

namespace CleanArch.Domain.Entities;

public sealed class PermissionClaim
{
    public string Type { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private PermissionClaim()
    {
    }

    public PermissionClaim(string type, string value)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(type), "Claim type is required");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(value), "Claim value is required");

        Type = type.Trim();
        Value = value.Trim();
    }
}
