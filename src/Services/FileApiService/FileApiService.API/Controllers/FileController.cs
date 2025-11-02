using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    IFileWorker _fileWorker;
    IItemRepository _itemRepository;
    public FileController(IFileWorker fileWorker,
        IItemRepository itemRepository)
    {
        _fileWorker = fileWorker;
        _itemRepository = itemRepository;
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
    //for getting file jsons
    [HttpGet]
    [Route("getitemsfromroot")]
    public ActionResult<List<Item>> GetItemsFromRoot([FromHeader(Name = "Id")] Guid userId)
    {
        var items = _itemRepository.GetRootItems(userId);
        var result = items.OfType<Item>().ToList();
        if(result.Count == 0)
            return NotFound();
        return result;
    }
    

}