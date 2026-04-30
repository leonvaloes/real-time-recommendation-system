using UserAuth.CleanArch.Domain.Abstractions;
using UserAuth.CleanArch.Domain.Validation;
using UserAuth.CleanArch.Domain.ValueObjects;

namespace UserAuth.CleanArch.Domain.Entities;

public sealed class NaturalPerson : IPerson
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Cpf? CPF { get; private set; }

    private NaturalPerson()
    {
    }

    public NaturalPerson(string firstName, string lastName, string? cpf)
    {
        ValidateDomain(firstName, lastName);

        Id = Guid.NewGuid();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        CPF = string.IsNullOrWhiteSpace(cpf) ? null : new Cpf(cpf);
    }

    public static NaturalPerson Restore(Guid id, string firstName, string lastName, string? cpf)
    {
        var person = new NaturalPerson(firstName, lastName, cpf)
        {
            Id = id
        };

        return person;
    }

    public string GetIdentifier()
    {
        return CPF?.Value ?? string.Empty;
    }

    public string GetDisplayName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    private static void ValidateDomain(string firstName, string lastName)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(firstName), "FirstName is required");
        DomainExceptionValidation.When(firstName.Length < 2, "FirstName too short");
        DomainExceptionValidation.When(firstName.Length > 100, "FirstName too long");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(lastName), "LastName is required");
        DomainExceptionValidation.When(lastName.Length < 2, "LastName too short");
        DomainExceptionValidation.When(lastName.Length > 100, "LastName too long");
    }
}
