using UserAuth.CleanArch.Domain.Entities;
using UserAuth.CleanArch.Domain.Validation;
using FluentAssertions;

namespace UserAuth.CleanArch.Domain.Tests;

public class UserTests
{
    [Fact]
    public void CreateUser_ShouldCreateNaturalPersonUser()
    {
        var person = new NaturalPerson("Carlos", "Silva", "12345678901");
        var user = new User("CARLOS@EMAIL.COM", "hashed-password", person, "(18)99745-0885");

        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("carlos@email.com");
        user.GetUserDisplayName().Should().Be("Carlos Silva");
        user.GetUserIdentifier().Should().Be("12345678901");
    }

    [Fact]
    public void CreateNaturalPerson_ShouldThrowDomainException_WhenCpfIsInvalid()
    {
        Action act = () => new NaturalPerson("Carlos", "Silva", "123");

        act.Should().Throw<DomainExceptionValidation>()
            .WithMessage("CPF is invalid");
    }

    [Fact]
    public void CreateJuridicalPerson_ShouldUseBusinessNameAsDisplayName()
    {
        var person = new JuridicalPerson("12345678000190", "Empresa LTDA", "Empresa");
        var user = new User("contato@empresa.com", "hashed-password", person);

        user.GetUserDisplayName().Should().Be("Empresa");
        user.GetUserIdentifier().Should().Be("12345678000190");
    }

    [Fact]
    public void AddRole_ShouldNotDuplicateRole()
    {
        var person = new NaturalPerson("Carlos", "Silva", null);
        var user = new User("carlos@email.com", "hashed-password", person);

        user.AddRole(new Role("Admin"));
        user.AddRole(new Role("Admin"));

        user.Roles.Should().ContainSingle(role => role.Name == "Admin");
    }
}
