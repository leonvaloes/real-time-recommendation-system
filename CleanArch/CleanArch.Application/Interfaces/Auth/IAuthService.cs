using CleanArch.Application.DTOs.Auth;

namespace CleanArch.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
}
