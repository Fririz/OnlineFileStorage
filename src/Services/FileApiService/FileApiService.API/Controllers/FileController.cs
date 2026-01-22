using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using FileApiService.Domain.Entities;
using FluentResults;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileWorker _fileWorker;
    protected Guid CurrentUserId
    {
        get
        {
            if (Guid.TryParse(Request.Headers["Id"].FirstOrDefault(), out var id))
            {
                return id;
            }

            throw new UnauthorizedAccessException();
        }
    }
    public FileController(IFileWorker fileWorker)
    {
        _fileWorker = fileWorker;
        
    }
    /// <summary>
    /// Get parent of file
    /// </summary>
    /// <param name="id">fileId</param>
    /// <returns></returns>
    [HttpGet("getparent/{id:guid}")]
    public async Task<ActionResult<Item>> GetParent(Guid id)
    {
        var parent = await _fileWorker.GetParent(id);
        
        if (parent is null)
        {
            return NotFound();
        }

        return Ok(parent);
    }
    /// <summary>
    /// Create file
    /// </summary>
    /// <param name="itemCreate"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("uploadfile")]
    public async Task<ActionResult> UploadFile(
        [FromBody] ItemCreateDto itemCreate, 
        CancellationToken cancellationToken)
    {
        var result = await _fileWorker.CreateFile(itemCreate, CurrentUserId, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(new { uploadUrl = result.Value });
        }

        return HandleError(result.Errors);
    }
    /// <summary>
    /// Download file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("downloadfile/{id:guid}")]
    public async Task<ActionResult> DownloadFile(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var result = await _fileWorker.DownloadFile(id, CurrentUserId, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(new { downloadUrl = result.Value });
        }

        return HandleError(result.Errors);
    }
/// <summary>
/// Delete file
/// </summary>
/// <param name="fileId"></param>
/// <param name="userId"></param>
/// <returns></returns>
    [HttpDelete("deletefile/{id:guid}")]
    public async Task<ActionResult> DeleteFile(Guid itemId)
    {
        var result = await _fileWorker.DeleteFile(itemId, CurrentUserId);
            
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleError(result.Errors);
    }
/// <summary>
/// Get all items from root
/// </summary>
/// <param name="userId">User id</param>
/// <returns>list of items</returns>
    [HttpGet("getitemsfromroot")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetItemsFromRoot()
    {
        var result = await _fileWorker.GetRootItems(CurrentUserId);
            
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleError(result.Errors);
    }
    private ActionResult HandleError(IReadOnlyList<IError> errors)
    {
        var error = errors.FirstOrDefault();
        
        if (error is null)
        {
            return StatusCode(500);
        }

        return error switch
        {
            FileNotFoundError => NotFound(new { error.Message }),
            UnauthorizedAccessError => Unauthorized(new { error.Message }),
            InvalidTypeOfItemError => BadRequest(new { error.Message }),
            InvalidParentError => BadRequest(new { error.Message }),
            _ => BadRequest(new { error.Message })
        };
    }

}