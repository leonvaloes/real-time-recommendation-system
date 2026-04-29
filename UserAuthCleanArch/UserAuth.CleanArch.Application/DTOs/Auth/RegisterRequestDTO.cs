namespace UserAuth.CleanArch.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PersonType { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CPF { get; set; }
    public string? CNPJ { get; set; }
    public string? RegisteredName { get; set; }
    public string? BusinessName { get; set; }
}
