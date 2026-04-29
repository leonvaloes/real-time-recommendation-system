using UserAuth.CleanArch.Application.DTOs.Auth;
using UserAuth.CleanArch.Application.Interfaces.Auth;
using UserAuth.CleanArch.Domain.Validation;
using Microsoft.AspNetCore.Mvc;

namespace UserAuth.CleanArch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Created(string.Empty, response);
        }
        catch (DomainExceptionValidation exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (DomainExceptionValidation exception)
        {
            return Unauthorized(new { message = exception.Message });
        }
    }
}
