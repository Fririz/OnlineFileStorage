using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    [HttpGet]
    [Route("GetFile")]
    public ActionResult<string> GetFile()
    {
        //return Ok("File");
        string id = HttpContext.Request.Headers["Id"];
        string name = HttpContext.Request.Headers["Nickname"];
        return id + " " + name;
    }
}