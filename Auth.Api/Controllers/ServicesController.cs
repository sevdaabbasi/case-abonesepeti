using System.Security.Claims;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{

    [Authorize(Roles = "User")]
    [HttpGet("service1")]
    public IActionResult Service1() => Ok(new { message = "User-only service" });


    [Authorize(Roles = "Admin")]
    [HttpGet("service2")]
    public IActionResult Service2() => Ok(new { message = "Admin-only service" });

    [Authorize]
    [HttpGet("service3")]
    public IActionResult Service3()
    {
        var phone = User.Claims.FirstOrDefault(c => c.Type == "phone")?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return Ok(new { message = $"Service3'e {role} olarak eriştin", phone });
    }

    [AllowAnonymous]
    [HttpGet("service4")]
    public IActionResult Service4() => Ok(new { message = "Public service4" });
}
