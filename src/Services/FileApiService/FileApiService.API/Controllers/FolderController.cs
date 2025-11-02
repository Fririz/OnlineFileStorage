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
    public async Task<ActionResult> CreateFolder(ItemDto item)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        await _folderWorker.CreateFolder(item, ownerId);
        return Ok();
    }
    [HttpGet]
    [Route("getallchildren/{id}/")]
    public ActionResult<List<Item>> GetAllChildren(Guid id)
    {
        var items = _itemRepository.GetAllChildren(id);
        var result = items.OfType<Item>().ToList();
        if(result.Count == 0)
            return NotFound();
        return result;
    }
    
}