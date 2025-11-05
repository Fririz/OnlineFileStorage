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
    private readonly IPublishEndpoint _publishEndpoint; 
    public FileController(IFileWorker fileWorker,
        IItemRepository itemRepository, IPublishEndpoint publishEndpoint)
    {
        _fileWorker = fileWorker;
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    [Route("getparent/{id}")]
    public ActionResult<Item> GetParent(Guid id)
    {
        var parent = _itemRepository.GetParent(id);
        return Ok(parent);
    }
    [HttpPost]
    [Route("uploadfile")]
    public async Task<ActionResult> UploadFile(ItemDto item)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        var link = await _fileWorker.CreateFile(item, ownerId);
        return Ok(new { uploadUrl = link });
    }
    [HttpGet]
    [Route("downloadfile/{id:minlength(1)}/")]
    public async Task<ActionResult> DownloadFile(Guid id)
    {
        var link = await _fileWorker.DownloadFile(id);
        return Ok(new { uploadUrl = link });
    }

    [HttpDelete]
    [Route("deletefile/{fileId}")]
    public async Task<ActionResult> DeleteFile(Guid fileId)
    {
        try
        {
            await _fileWorker.DeleteFile(fileId);
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
    [HttpGet]
    [Route("getitemsfromroot")]
    public ActionResult<List<Item>> GetItemsFromRoot([FromHeader(Name = "Id")] Guid userId)
    {
        var items = _itemRepository.GetRootItems(userId);
        var result = items.OfType<Item>().ToList();
        return result;
    }
    

}