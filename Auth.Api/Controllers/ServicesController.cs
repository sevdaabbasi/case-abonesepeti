using Auth.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AuthService _auth;

    public ServicesController(AuthService auth)
    {
        _auth = auth;
    }

    // Helper: headerlerden phone/password al ve doğrula
    private async Task<(bool Success, string? Message, Models.User? User)> AuthenticateFromHeaders()
    {
        if (!Request.Headers.TryGetValue("X-Phone", out var phone) ||
            !Request.Headers.TryGetValue("X-Password", out var password))
            return (false, "X-Phone veya X-Password header'ları eksik.", null);

        var user = await _auth.ValidateCredentialsAsync(phone, password);
        if (user == null) return (false, "Kimlik doğrulama başarısız.", null);
        return (true, null, user);
    }

    // 1. servis — sadece User rolü
    [HttpGet("service1")]
    public async Task<IActionResult> Service1()
    {
        var (ok, msg, user) = await AuthenticateFromHeaders();
        if (!ok) return Unauthorized(new { message = msg });

        if (user!.Role != Models.Role.User)
            return Forbid();

        return Ok(new { message = "Service1'e User olarak eriştin.", phone = user.Phone });
    }

    // 2. servis — sadece Admin rolü
    [HttpGet("service2")]
    public async Task<IActionResult> Service2()
    {
        var (ok, msg, user) = await AuthenticateFromHeaders();
        if (!ok) return Unauthorized(new { message = msg });

        if (user!.Role != Models.Role.Admin)
            return Forbid();

        return Ok(new { message = "Service2'ye Admin olarak eriştin.", phone = user.Phone });
    }

    // 3. servis — her iki rol
    [HttpGet("service3")]
    public async Task<IActionResult> Service3()
    {
        var (ok, msg, user) = await AuthenticateFromHeaders();
        if (!ok) return Unauthorized(new { message = msg });

        return Ok(new { message = $"Service3'e {user!.Role} rolü ile eriştin.", phone = user.Phone });
    }

    // 4. servis — public (token/headers olmadan)
    [HttpGet("service4")]
    public IActionResult Service4()
    {
        return Ok(new { message = "Public service4 — herkese açık." });
    }
}
