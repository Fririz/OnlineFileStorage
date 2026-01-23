using FileStorageService.Application.Contracts;
using FileStorageService.Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;

[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    private readonly IFileManager _fileManager;

    public FileController(IFileManager fileManager)
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
    [HttpGet]
    [Route("download/{id}/{filename:minlength(1)}/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFile(Guid id, string filename, string? token, CancellationToken cancellationToken = default)
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
    [HttpPost]
    [Route("upload/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UploadFile(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _fileManager.UploadFileCaseAsync(Request.Body, id, cancellationToken);

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