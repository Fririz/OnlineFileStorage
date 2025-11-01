using FileStorageService.Application.Contracts;
using FileStorageService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;
[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    private readonly IFileManager _fileManager;
    public FileController(IFileManager fileRepository)
    {
        _fileManager = fileRepository;
    }

    [HttpGet]
    [Route("download/{id}/{filename:minlength(1)}/{token}")]
    public async Task<IActionResult> GetFile(Guid id, string filename, string? token)
    {
        try
        {
            var stream = await _fileManager.DownloadFileCaseAsync(id, token);
            return File(stream, "application/octet-stream", filename);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized();
        }
        catch (FileNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    

    [HttpPost]
    [Route("upload/{id}")]
    public async Task<ActionResult<string>> UploadFile([FromForm] IFormFile? file, Guid id)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Empty file.");
        }

        await using var input = file.OpenReadStream();
        await _fileManager.UploadFileCaseAsync(input, id);
        return Ok("File uploaded successfully");
    }

}