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

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Ok message</returns>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(UserAuthDto user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _userWorker.RegisterUser(user, cancellationToken);
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
        _logger.LogInformation("User {Username} registered successfully.", user.Username);
        
        return Ok(new { message = "Registration successful" });
    }
    /// <summary>
    /// Login a registered user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Ok message</returns>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserAuthDto user, CancellationToken cancellationToken = default)
    {
        var token = await _userWorker.LoginUser(user, cancellationToken);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
        
        Response.Cookies.Append("token", token, GetCookieOptions());
        
        _logger.LogInformation("User {Username} logged in successfully.", user.Username);
        
        return Ok(new { message = "Login successful" });
    }
    /// <summary>
    /// Logout a registered user
    /// </summary>
    /// <returns>Ok message</returns>
    [Authorize] 
    [HttpGet]
    [Route("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        var username = User.Identity?.Name ?? "unknown";
        Response.Cookies.Delete("token");
        _logger.LogInformation($"User {username} logged out successfully.");
        return Ok(new { message = $"Logout successful. User: {username}" });
    }
    /// <summary>
    /// Get current user from coockie
    /// </summary>
    /// <returns>user info</returns>
    [HttpGet]
    [Route("GetCurrentUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            Expires = DateTime.UtcNow.AddDays(10), 
            Path = "/", 
        };
    }
}
