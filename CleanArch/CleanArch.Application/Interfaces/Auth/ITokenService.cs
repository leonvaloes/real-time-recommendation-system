using CleanArch.Domain.Entities;

namespace CleanArch.Application.Interfaces.Auth;

public interface ITokenService
{
    string Generate(User user);
}
