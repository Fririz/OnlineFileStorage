using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.Entities;
using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using IdentityService.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Application.Exceptions.FluentResultsErrors;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserWorker _userWorker;
    private readonly IJwtTokenWorker _jwtTokenWorker;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Auth controller
    /// </summary>
    /// <param name="userWorker"></param>
    /// <param name="jwtTokenWorker"></param>
    /// <param name="logger"></param>
    public AuthController(IUserWorker userWorker, IJwtTokenWorker jwtTokenWorker, ILogger<AuthController> logger)
    {
        _userWorker = userWorker;
        _jwtTokenWorker = jwtTokenWorker;
        _logger = logger;
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
        var registrationResult = await _userWorker.RegisterUser(user, cancellationToken);
        if (registrationResult.IsFailed)
        {
            var error = registrationResult.Errors.First();
            _logger.LogWarning("Registration failed: {Message}", error.Message);
            return error switch
            {
                UserAlreadyExistsError => Conflict(new { error = error.Message }),
                _ => BadRequest(new { error = error.Message })
            };
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
        var loginResult = await _userWorker.LoginUser(user, cancellationToken);
        if (loginResult.IsFailed)
        {
            var error = loginResult.Errors.First();
            _logger.LogInformation("Login failed: {Message}", error.Message);
            return error switch
            {
                InvalidUserDataException => Unauthorized(new { error = error.Message }),
                _ => BadRequest(new { error = error.Message })
            };
        }

        var token = loginResult.Value;
        
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
