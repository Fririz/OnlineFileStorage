using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using IdentityService.Application.Exceptions.FluentResultsErrors;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IUserWorker _userWorker;

    public SessionsController(IUserWorker userWorker)
    {
        _userWorker = userWorker;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserAuthDto user, CancellationToken cancellationToken = default)
    {
        var loginResult = await _userWorker.LoginUser(user, cancellationToken);
        if (loginResult.IsFailed)
        {
            var error = loginResult.Errors.First();
            return error switch
            {
                InvalidUserDataError => Unauthorized(new { error = error.Message }),
                _ => BadRequest(new { error = error.Message })
            };
        }

        var token = loginResult.Value;
        
        Response.Cookies.Append("token", token, GetCookieOptions());
        
        return Ok(new { message = "Login successful" });
    }
    [HttpDelete] 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {

        Response.Cookies.Delete("token");
        
        return NoContent(); 
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