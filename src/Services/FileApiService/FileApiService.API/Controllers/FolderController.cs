using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderWorker _folderWorker;

    public FolderController(IFolderWorker fileWorker)
    {
        _folderWorker = fileWorker;
    }

    [HttpPost]
    [Route("createfolder")]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        if (!Guid.TryParse(Request.Headers["Id"].ToString(), out var ownerId))
        {
            return Unauthorized("User ID header is missing or invalid");
        }

        var result = await _folderWorker.CreateFolder(itemCreate, ownerId);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleErrors(result.Errors);
    }

    [HttpGet]
    [Route("getallchildren/{id}/")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetAllChildren(Guid id)
    {
        var result = await _folderWorker.GetChildrenAsync(id);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleErrors(result.Errors);
    }

    [HttpDelete]
    [Route("deletefolder/{id}/")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid id)
    {
        if (!Guid.TryParse(Request.Headers["Id"].ToString(), out var ownerId))
        {
            return Unauthorized("User ID header is missing or invalid");
        }

        var result = await _folderWorker.DeleteFolderWithAllChildren(id, ownerId);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleErrors(result.Errors);
    }

    private ActionResult HandleErrors(IReadOnlyList<IError> errors)
    {
        var firstError = errors.FirstOrDefault();

        if (firstError == null)
        {
            return StatusCode(500, "Unknown error");
        }

        return firstError switch
        {
            DirectoryNotFoundError => NotFound(firstError.Message),               
            UnauthorizedAccessError => Unauthorized(firstError.Message),  
            InvalidTypeOfItemError => BadRequest(firstError.Message),
            _ => StatusCode(500, firstError.Message)                      
        };
    }
}