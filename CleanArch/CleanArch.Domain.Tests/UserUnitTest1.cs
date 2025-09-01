using CleanArch.Domain.Entities;
using CleanArch.Domain.Validation;
using FluentAssertions;

namespace CleanArch.Domain.Tests
{
    public class UserUnitTest1
    {
        [Fact]
        public void CreateUser_ShouldCreateValidUser()
        {
            var user = new User("carlos123", "carlos@email.com", "(18)99745-0885");
            // Assert
            user.Should().NotBeNull();
            user.Email.Should().NotBeNull();
            user.Name.Should().Be("carlos123");
        }

        [Fact]
        public void CreateUser_WithInvalidUsernameNaturalPerson_ShouldThrowDomainException()
        {

            Action act = () => new User(" ", "carlos@email.com", "(18)99745-0885");

            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Name is required");
        }

    }
}