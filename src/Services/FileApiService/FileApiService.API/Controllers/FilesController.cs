using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : BaseApiController
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
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
    [HttpPost]
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
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{fileId:guid}")]
    public async Task<ActionResult> DownloadFile(
        Guid fileId, 
        CancellationToken cancellationToken)
    {
        var result = await _fileService.DownloadFile(fileId, CurrentUserId, cancellationToken);
        
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
    /// <returns></returns>
    [HttpDelete("{fileId:guid}")]
    public async Task<ActionResult> DeleteFile(Guid fileId)
    {
        var result = await _fileService.DeleteFile(fileId, CurrentUserId);
            
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleResult(result);
    }


}