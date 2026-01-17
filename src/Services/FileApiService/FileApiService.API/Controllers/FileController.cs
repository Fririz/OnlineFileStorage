using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using FileApiService.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileWorker _fileWorker;
    public FileController(IFileWorker fileWorker)
    {
        _fileWorker = fileWorker;
    }
    /// <summary>
    /// Get parent of file
    /// </summary>
    /// <param name="id">fileId</param>
    /// <returns></returns>
    [HttpGet]
    [Route("getparent/{id}")]
    public async Task<ActionResult<Item>> GetParent(Guid id)
    {
        var parent = await _fileWorker.GetParent(id);
        return Ok(parent);
    }
    /// <summary>
    /// Create file
    /// </summary>
    /// <param name="itemCreate"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("uploadfile")]
    public async Task<ActionResult> UploadFile(ItemCreateDto itemCreate, [FromHeader(Name = "Id")] Guid userId, CancellationToken cancellationToken = default)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        var result = await _fileWorker.CreateFile(itemCreate, ownerId, cancellationToken);
        if (result.IsFailed)
        {
            var error = result.Errors.First();
            return error switch
            {
                _ => BadRequest(new {error.Message})
            };
        }
        return Ok(new { uploadUrl = result.Value });
    }
    /// <summary>
    /// Download file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("downloadfile/{id:minlength(1)}/")]
    public async Task<ActionResult> DownloadFile(Guid id, [FromHeader(Name = "Id")] Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _fileWorker.DownloadFile(id, userId, cancellationToken);
        if (result.IsFailed)
        {
            var error = result.Errors.First();
            return error switch
            {
                FileNotFoundError => NotFound(new { error.Message }),
                UnauthorizedAccessError => Unauthorized(new {error.Message}),
                _ => BadRequest(new {error.Message})
            };
        }
        return Ok(new { uploadUrl = result.Value });
    }
/// <summary>
/// Delete file
/// </summary>
/// <param name="fileId"></param>
/// <param name="userId"></param>
/// <returns></returns>
    [HttpDelete]
    [Route("deletefile/{fileId}")]
    public async Task<ActionResult> DeleteFile(Guid fileId, [FromHeader(Name = "Id")] Guid userId)
    {
        var result = await _fileWorker.DeleteFile(fileId, userId);
        if (result.IsFailed)
        {
            var error = result.Errors.First();
            return error switch
            {
                FileNotFoundError => NotFound(new {error.Message}),
                InvalidOperationError => BadRequest(new {error.Message}),
                UnauthorizedAccessError => Unauthorized( new {error.Message}),
                _ => BadRequest(new {error.Message})
            };
        }
        return Ok();
    }
/// <summary>
/// Get all items from root
/// </summary>
/// <param name="userId">User id</param>
/// <returns>list of items</returns>
    [HttpGet]
    [Route("getitemsfromroot")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetItemsFromRoot([FromHeader(Name = "Id")] Guid userId)
    {
        var items = await _fileWorker.GetRootItems(userId);
        return items;
    }
    

}