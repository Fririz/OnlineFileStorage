using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using IdentityService.Application.Exceptions.FluentResultsErrors;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UsersController : ControllerBase{
    private readonly IUserWorker _userWorker;

    public UsersController(IUserWorker userWorker)
    {
        _userWorker = userWorker;
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(UserAuthDto user, CancellationToken cancellationToken = default)
    {
        var registrationResult = await _userWorker.RegisterUser(user, cancellationToken);
        if (registrationResult.IsFailed)
        {
            var error = registrationResult.Errors.First();
            return error switch
            {
                UserAlreadyExistsError => Conflict(new { error = error.Message }),
                _ => BadRequest(new { error = error.Message })
            };
        }
        return Ok(new { message = "Registration successful" });
    }
    
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        string? token = Request.Cookies["token"];
        if (token == null)
        {
            return Unauthorized();
        }
        
        var userResult = _userWorker.GetCurrentUser(token);
        if (userResult.IsFailed)
        {
            var error = userResult.Errors.First();
            return error switch
            {
                InvalidTokenError => Unauthorized(new { error = error.Message }),
                _ => BadRequest(new { error = error.Message })
            };
        }
        UserTokenCheckDto user =  userResult.Value;
        return Ok(user);
    }
}