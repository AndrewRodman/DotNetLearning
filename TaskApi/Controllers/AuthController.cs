using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Services;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request);
        if (response is null)
        {
            return Conflict(new { message = "Username already exists." });
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        if (response is null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(response);
    }
}