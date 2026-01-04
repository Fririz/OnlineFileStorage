using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileWorker _fileWorker;
    private readonly IItemRepository _itemRepository;
    public FileController(IFileWorker fileWorker,
        IItemRepository itemRepository)
    {
        _fileWorker = fileWorker;
        _itemRepository = itemRepository;
    }
    /// <summary>
    /// Get parent of file
    /// </summary>
    /// <param name="id">fileId</param>
    /// <returns></returns>
    [HttpGet]
    [Route("getparent/{id}")]
    public ActionResult<Item> GetParent(Guid id)
    {
        var parent = _itemRepository.GetParent(id);
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
        var link = await _fileWorker.CreateFile(itemCreate, ownerId, cancellationToken);
        return Ok(new { uploadUrl = link });
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
        var link = await _fileWorker.DownloadFile(id, userId, cancellationToken);
        return Ok(new { uploadUrl = link });
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
        try
        {
            await _fileWorker.DeleteFile(fileId, userId);
            return Ok();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
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