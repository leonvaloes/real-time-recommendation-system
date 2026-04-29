using System.Security.Claims;
using System.Text;
using CleanArch.Application.Interfaces.Auth;
using CleanArch.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArch.Infra.IoC.Security;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate(User user)
    {
        var issuer = GetConfigurationValue("JWT_ISSUER", "CleanArch.API");
        var audience = GetConfigurationValue("JWT_AUDIENCE", "CleanArch.Client");
        var secret = GetRequiredConfigurationValue("JWT_SECRET");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.GetUserDisplayName()),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));

            foreach (var permission in role.Claims)
            {
                claims.Add(new Claim(permission.Type, permission.Value));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private string GetConfigurationValue(string key, string fallback)
    {
        return Environment.GetEnvironmentVariable(key)
            ?? _configuration[key]
            ?? fallback;
    }

    private string GetRequiredConfigurationValue(string key)
    {
        var value = Environment.GetEnvironmentVariable(key) ?? _configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} must be configured.");
        }

        return value;
    }

}
