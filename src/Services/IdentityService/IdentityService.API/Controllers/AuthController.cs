using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public IActionResult Register()
    {
        return Ok();
    }
    [HttpPost]
    [Route("login")]
    public IActionResult Login()
    {
        return Ok();
    }

    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        return Ok();
    }
    
}