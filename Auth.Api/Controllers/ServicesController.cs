using System.Security.Claims;
using Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{

    [Authorize(Roles = "User")]
    [HttpGet("user")]
    public IActionResult Service1()
    {
        var phone = User.Claims.FirstOrDefault(c => c.Type == "Phone")?.Value;
        return Ok(new { message = "User token 'ı ile istek atıldı" , phone});

    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Service2()
    {
        var phone = User.Claims.FirstOrDefault(c => c.Type == "Phone")?.Value;
        return Ok(new { message = $"User token 'ı ile istek atıldı", phone});
    } 

    [Authorize]
    [HttpGet("user,admin")]
    public IActionResult Service3()
    {
        var phone = User.Claims.FirstOrDefault(c => c.Type == "Phone")?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return Ok(new { message = $"Service'e {role} olarak eriştin", phone });
    }

    [AllowAnonymous]
    [HttpGet("herkes")]
    public IActionResult Service4() => Ok(new { message = "Bu servise erişmek için token gerekmez..." });
}
