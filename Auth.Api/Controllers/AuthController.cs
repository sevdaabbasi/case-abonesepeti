using Auth.Api.Dtos.Requests;
using Auth.Api.Models;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
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
    
    [AllowAnonymous]
    [HttpPost("register")]
    
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var (success, message, user, tokens) = await _auth.RegisterAsync(req);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var (success, message, user, tokens) = await _auth.LoginAsync(req);
        if (!success) return Unauthorized(new { message });
        return Ok(new {message , tokens});
    }
   
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var (success, message, tokens) = await _auth.RefreshAsync(req);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, tokens });
    }
}