using FileStorageService.Application.Contracts;
using FileStorageService.Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileManager _fileManager;

    public FilesController(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    /// <summary>
    /// Download file
    /// </summary>
    /// <param name="id">id in storage</param>
    /// <param name="filename">filename for user</param>
    /// <param name="token">jwt token from link</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}/{filename}/{token}")] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFile(
        [FromRoute] Guid id, 
        [FromRoute] string? token, 
        [FromRoute] string filename,
        CancellationToken cancellationToken = default)
    {
        var result = await _fileManager.DownloadFileCaseAsync(id, token, cancellationToken);

        if (result.IsSuccess)
        {
            return File(result.Value, "application/octet-stream", filename);
        }

        if (result.HasError<UnauthorizedError>())
        {
            return Unauthorized();
        }

        if (result.HasError<FileNotFoundError>())
        {
            return NotFound();
        }

        return StatusCode(500, result.Errors.FirstOrDefault()?.Message);
    }
    
    /// <summary>
    /// Upload file
    /// </summary>
    /// <param name="id">File id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadFile(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        long length = Request.ContentLength ?? 0;
        if (length <= 0)
        {
            return BadRequest("Content-Length header is missing or zero.");
        }

        string contentType = Request.ContentType ?? "application/octet-stream";

        var result = await _fileManager.UploadFileCaseAsync(Request.Body, length, contentType, id, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok("File uploaded successfully");
        }

        if (result.HasError<FileUploadError>())
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        return StatusCode(500, result.Errors.FirstOrDefault()?.Message);
    }
}