using Auth.Api.Models;
using Auth.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var (success, message, user) = await _auth.RegisterAsync(req);
        if (!success) return BadRequest(new { message });
        return Ok(new AuthResponse(user!.Phone, user.Role.ToString(), message));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var (success, message, user) = await _auth.LoginAsync(req);
        if (!success) return Unauthorized(new { message });
        return Ok(new AuthResponse(user!.Phone, user.Role.ToString(), message));
    }
}