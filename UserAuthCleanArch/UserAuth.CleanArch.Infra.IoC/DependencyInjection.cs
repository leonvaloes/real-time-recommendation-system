namespace UserAuth.CleanArch.Infra.IoC;

using UserAuth.CleanArch.Application.Interfaces;
using UserAuth.CleanArch.Application.Interfaces.Auth;
using UserAuth.CleanArch.Application.Mappings;
using UserAuth.CleanArch.Application.Services;
using UserAuth.CleanArch.Domain.Interfaces;
using UserAuth.CleanArch.Infra.Data.Context;
using UserAuth.CleanArch.Infra.Data.Repositories;
using UserAuth.CleanArch.Infra.IoC.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<MySqlContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<CreateUserMapping>();

        return services;
    }
}
