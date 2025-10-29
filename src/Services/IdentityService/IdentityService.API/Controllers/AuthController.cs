using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.Entities;
using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using IdentityService.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserWorker _userWorker;
    private readonly IJwtTokenWorker _jwtTokenWorker;
    private readonly ILogger _logger;
    
    private readonly IHostEnvironment _env;

    public AuthController(IUserWorker userWorker, IJwtTokenWorker jwtTokenWorker, ILogger<AuthController> logger, IHostEnvironment env)
    {
        _userWorker = userWorker;
        _jwtTokenWorker = jwtTokenWorker;
        _logger = logger;
        _env = env; 
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(UserAuthDto user)
    {
        try
        {
            await _userWorker.RegisterUser(user);
        }
        catch (UserAlreadyExistsException ex)
        {
            return BadRequest(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error with user registration");
            return StatusCode(500, new { message = "An internal server error occurred." });
        }

        var token = await _userWorker.LoginUser(user);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid credentials after registration." });
        }
        
        
        Response.Cookies.Append("token", token, GetCookieOptions());
        
        _logger.LogInformation("User {Username} registered and logged in successfully.", user.Username);
        
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto user)
    {
        var token = await _userWorker.LoginUser(user);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
        
        Response.Cookies.Append("token", token, GetCookieOptions());
        
        _logger.LogInformation("User {Username} logged in successfully.", user.Username);
        
        return Ok(new { message = "Login successful" });
    }
    
    [Authorize] 
    [HttpGet]
    [Route("logout")]
    public IActionResult Logout()
    {
        var username = User.Identity?.Name ?? "unknown";
        Response.Cookies.Delete("token");
        _logger.LogInformation($"User {username} logged out successfully.");
        return Ok(new { message = $"Logout successful. User: {username}" });
    }

    [HttpGet]
    [Route("GetCurrentUser")]
    public IActionResult GetCurrentUser()
    {
        string? token = Request.Cookies["token"];

        var principal = _jwtTokenWorker.GetPrincipalFromToken(token);

        if (principal == null)
        {
            return Unauthorized();
        }
        var user = new UserTokenCheckDto()
        {
            Id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
            Username = principal.FindFirstValue(ClaimTypes.Name) 
        };

        if (user.Id == null || user.Username == null)
        {
            _logger.LogError("Token validated but required claims (sub, name) are missing.");
            return Unauthorized();
        }

        return Ok(user);
    }
    private CookieOptions GetCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = false,
            Expires = DateTime.UtcNow.AddDays(1), 
            Path = "/", 
        };
    }
}
