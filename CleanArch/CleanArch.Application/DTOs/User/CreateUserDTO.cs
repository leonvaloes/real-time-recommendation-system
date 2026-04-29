namespace CleanArch.Application.DTOs;

public class CreateUserDTO
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CPF { get; set; }
    public string? Phone { get; set; }
}
