using UserAuth.CleanArch.Domain.Entities;

namespace UserAuth.CleanArch.Application.Interfaces.Auth;

public interface ITokenService
{
    string Generate(User user);
}
