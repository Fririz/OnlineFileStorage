using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : BaseApiController
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService, IItemService itemService) : base(itemService)
    {
        _fileService = fileService;
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
        var result = await _fileService.CreateFile(itemCreate, CurrentUserId, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(new { uploadUrl = result.Value });
        }

        return HandleResult(result);
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
        var result = await _fileService.DownloadFile(id, CurrentUserId, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(new { downloadUrl = result.Value });
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("deletefile/{itemId:guid}")]
    public async Task<ActionResult> DeleteFile(Guid itemId)
    {
        var result = await _fileService.DeleteFile(itemId, CurrentUserId);
            
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get all items from root
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>list of items</returns>
    [HttpGet("getitemsfromroot")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetItemsFromRoot()
    {
        var result = await _fileService.GetRootItems(CurrentUserId);
        return HandleResult(result);
    }
}