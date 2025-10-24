using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.Entities;
using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserWorker _userWorker;
    private readonly IJwtTokenWorker _jwtTokenWorker;
    private readonly ILogger _logger;
    public AuthController(IUserWorker userWorker, IJwtTokenWorker jwtTokenWorker, ILogger<AuthController> logger)
    {
        _userWorker = userWorker;
        _jwtTokenWorker = jwtTokenWorker;
        _logger = logger;
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
        var token = await _userWorker.LoginUser(user);
        if (token == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,      // нужна для cross-site POST
            Path = "/",                        // чтобы уходила на все маршруты
        };
        Response.Cookies.Append("token", token, cookieOptions);
        return Ok(new { message = "Login successful" });
    }
    
    [Authorize]
    [HttpGet]
    [Route("logout")]
    public IActionResult Logout()
    {
 

        Response.Cookies.Delete("token");
        _logger.LogInformation($"User {User.Identity!.Name} logged out successfully.");
        return Ok(new { message = "Logout successful. User: " + User.Identity.Name });
    }
    [HttpGet]
    [Route("tokenCheck")]
    public bool TokenCheck()
    {
        string? token = Request.Cookies["token"];
        if (token != null)
            return _jwtTokenWorker.CheckToken(token);
        return false;
    }
    
}