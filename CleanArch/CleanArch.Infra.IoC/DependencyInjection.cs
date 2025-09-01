namespace CleanArch.Infra.IoC;

using CleanArch.Domain.Entities;
using CleanArch.Infra.Data.Context;
using CleanArch.Infra.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registra MongoContext como singleton
        services.AddSingleton<MongoContext>();

        // Registra repositórios
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
