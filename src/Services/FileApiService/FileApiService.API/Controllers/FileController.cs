using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    [HttpGet]
    [Route("GetFile/{id}")]
    public ActionResult<string> GetFile(Guid id)
    {
        return Ok("File");
        // string id = HttpContext.Request.Headers["Id"];
        // string name = HttpContext.Request.Headers["Nickname"];
        // return id + " " + name;
    }
}