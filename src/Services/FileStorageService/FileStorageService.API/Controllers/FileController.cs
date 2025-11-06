using FileStorageService.Application.Contracts;
using FileStorageService.Infrastructure;
using MassTransit;
using Contracts.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;
[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;
    private readonly IPublishEndpoint _publishEndpoint; 
    public FileController(IFileManager fileManager, 
        IPublishEndpoint publishEndpoint,
        IFileRepository fileRepository)
    {
        _fileManager = fileManager;
        _publishEndpoint = publishEndpoint;
        _fileRepository = fileRepository;
    }

    [HttpGet]
    [Route("download/{id}/{filename:minlength(1)}/{token}")]
    public async Task<IActionResult> GetFile(Guid id, string filename, string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await _fileManager.DownloadFileCaseAsync(id, token, cancellationToken);
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
    public async Task<ActionResult<string>> UploadFile(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _fileManager.UploadFileCaseAsync(Request.Body, id, cancellationToken);
            return Ok("File uploaded successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

    }

}