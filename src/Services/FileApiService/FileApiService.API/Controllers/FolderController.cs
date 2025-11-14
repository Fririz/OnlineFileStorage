using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderWorker _folderWorker;
    private readonly IItemRepository _itemRepository;
    public FolderController(IFolderWorker fileWorker,
        IItemRepository itemRepository)
    {
        _folderWorker = fileWorker;
        _itemRepository = itemRepository;
    }
    [HttpPost]
    [Route("createfolder")]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        await _folderWorker.CreateFolder(itemCreate, ownerId);
        return Ok();
    }
    [HttpGet]
    [Route("getallchildren/{id}/")]
    public async Task<ActionResult<List<Item>>> GetAllChildren(Guid id)
    {
        var items = await _itemRepository.GetAllChildrenAsync(id);
        var result = items.OfType<Item>().ToList();
        return result;
    }

    [HttpDelete]
    [Route("deletefolder/{id}/")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid id)
    {
        try
        {
            await _folderWorker.DeleteFolderWithAllChildren(id, Guid.Parse(Request.Headers["Id"].ToString()));
            return Ok();
        }

        catch (InvalidOperationException)
        {
            return BadRequest("Incorrect type of item");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (DirectoryNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}