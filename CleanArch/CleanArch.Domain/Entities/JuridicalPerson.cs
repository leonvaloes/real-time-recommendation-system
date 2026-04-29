using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Validation;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

public sealed class JuridicalPerson : IPerson
{
    public Guid Id { get; private set; }
    public Cnpj CNPJ { get; private set; } = null!;
    public string RegisteredName { get; private set; } = string.Empty;
    public string? BusinessName { get; private set; }

    private JuridicalPerson()
    {
    }

    public JuridicalPerson(string cnpj, string registeredName, string? businessName)
    {
        ValidateDomain(registeredName, businessName);

        Id = Guid.NewGuid();
        CNPJ = new Cnpj(cnpj);
        RegisteredName = registeredName.Trim();
        BusinessName = string.IsNullOrWhiteSpace(businessName) ? null : businessName.Trim();
    }

    public string GetIdentifier()
    {
        return CNPJ.Value;
    }

    public string GetDisplayName()
    {
        return BusinessName ?? RegisteredName;
    }

    private static void ValidateDomain(string registeredName, string? businessName)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(registeredName), "RegisteredName is required");
        DomainExceptionValidation.When(registeredName.Length < 2, "RegisteredName too short");
        DomainExceptionValidation.When(registeredName.Length > 150, "RegisteredName too long");

        if (!string.IsNullOrWhiteSpace(businessName))
        {
            DomainExceptionValidation.When(businessName.Length > 150, "BusinessName too long");
        }
    }
}
