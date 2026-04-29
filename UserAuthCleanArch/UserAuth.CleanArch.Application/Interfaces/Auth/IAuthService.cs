using UserAuth.CleanArch.Application.DTOs.Auth;

namespace UserAuth.CleanArch.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
}
