using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.Entities;
using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserWorker _userWorker;
    public AuthController(IUserWorker userWorker)
    {
        _userWorker = userWorker;
    }
    [HttpPost]
    [Route("register")]
    public async Task<IResult> Register(UserDto user)
    { 
        return await _userWorker.RegisterUser(user);
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserDto user)
    {
        var token = await _userWorker.Login(user);
        if (token == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        Response.Cookies.Append("token", token, cookieOptions);
        return Ok(new { message = "Login successful" });
    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { message = "Logout successful" });
    }
    
}