using CleanArch.Application.DTOs.Auth;
using CleanArch.Application.Interfaces.Auth;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.Validation;

namespace CleanArch.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        ValidatePassword(request.Password);

        var normalizedEmail = NormalizeEmail(request.Email);
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        DomainExceptionValidation.When(existingUser is not null, "Email already registered");

        var person = CreatePerson(request);
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(normalizedEmail, passwordHash, person, request.Phone);
        user.AddRole(new Role("User", new[]
        {
            new PermissionClaim("permission", "users:read")
        }));

        await _userRepository.CreateAsync(user);

        return CreateResponse(user);
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        DomainExceptionValidation.When(user is null, "Invalid email or password");
        DomainExceptionValidation.When(
            !_passwordHasher.Verify(request.Password, user!.PasswordHash),
            "Invalid email or password");

        user.RegisterActivity();
        await _userRepository.UpdateAsync(user);

        return CreateResponse(user);
    }

    private AuthResponseDTO CreateResponse(User user)
    {
        return new AuthResponseDTO
        {
            UserId = user.Id.ToString(),
            DisplayName = user.GetUserDisplayName(),
            Identifier = user.GetUserIdentifier(),
            Email = user.Email,
            Token = _tokenService.Generate(user)
        };
    }

    private static IPerson CreatePerson(RegisterRequestDTO request)
    {
        return request.PersonType.Trim().ToLowerInvariant() switch
        {
            "natural" => new NaturalPerson(
                request.FirstName ?? string.Empty,
                request.LastName ?? string.Empty,
                request.CPF),
            "juridical" => new JuridicalPerson(
                request.CNPJ ?? string.Empty,
                request.RegisteredName ?? string.Empty,
                request.BusinessName),
            _ => throw new DomainExceptionValidation("PersonType is invalid")
        };
    }

    private static string NormalizeEmail(string email)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(email), "Email is required");
        return email.Trim().ToLowerInvariant();
    }

    private static void ValidatePassword(string password)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(password), "Password is required");
        DomainExceptionValidation.When(password.Length < 8, "Password too short");
        DomainExceptionValidation.When(password.Length > 100, "Password too long");
    }
}
