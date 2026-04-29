using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Validation;

namespace CleanArch.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public PersonType PersonType { get; private set; }
    public NaturalPerson? NaturalPerson { get; private set; }
    public JuridicalPerson? JuridicalPerson { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastActivity { get; private set; }
    public List<Role> Roles { get; private set; } = new();

    private User()
    {
    }

    public User(string email, string passwordHash, IPerson person, string? phone = null)
    {
        ValidateDomain(email, passwordHash, phone);

        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        SetPerson(person);
    }

    public string GetUserDisplayName()
    {
        return GetPerson().GetDisplayName();
    }

    public string GetUserIdentifier()
    {
        return GetPerson().GetIdentifier();
    }

    public Guid GetPersonId()
    {
        return GetPerson().Id;
    }

    public void AddRole(Role role)
    {
        var alreadyExists = Roles.Any(existingRole => existingRole.Name == role.Name);
        if (!alreadyExists)
        {
            Roles.Add(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RegisterActivity()
    {
        LastActivity = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private IPerson GetPerson()
    {
        return PersonType switch
        {
            PersonType.Natural when NaturalPerson is not null => NaturalPerson,
            PersonType.Juridical when JuridicalPerson is not null => JuridicalPerson,
            _ => throw new InvalidOperationException("User person is not configured.")
        };
    }

    private void SetPerson(IPerson person)
    {
        switch (person)
        {
            case NaturalPerson naturalPerson:
                PersonType = PersonType.Natural;
                NaturalPerson = naturalPerson;
                JuridicalPerson = null;
                break;
            case JuridicalPerson juridicalPerson:
                PersonType = PersonType.Juridical;
                NaturalPerson = null;
                JuridicalPerson = juridicalPerson;
                break;
            default:
                throw new DomainExceptionValidation("Person type is invalid");
        }
    }

    private static void ValidateDomain(string email, string passwordHash, string? phone)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(email), "Email is required");
        DomainExceptionValidation.When(!email.Contains('@'), "Email is invalid");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(passwordHash), "PasswordHash is required");

        if (!string.IsNullOrWhiteSpace(phone))
        {
            DomainExceptionValidation.When(phone.Length < 8, "Phone too short");
            DomainExceptionValidation.When(phone.Length > 20, "Phone too long");
        }
    }
}
