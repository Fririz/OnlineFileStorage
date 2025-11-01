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
    [HttpPost]
    [Route("uploadfile")]
    public async Task<ActionResult<string>> UploadFile(ItemDto item)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        var link = await _fileWorker.CreateFile(item, ownerId);
        return link;
    }
    [HttpGet]
    [Route("downloadfile/{id:minlength(1)}/")]
    public async Task<ActionResult<string>> DownloadFile(Guid id)
    {
        var link = await _fileWorker.DownloadFile(id);
        return link;
    }
    //for getting file jsons
    [HttpGet]
    [Route("getitemsfromroot")]
    public async Task<ActionResult<IReadOnlyList<Item>>> GetItemsFromRoot()
    {
        var userId = Request.Headers["Id"].ToString();
        _itemRepository.GetRootItems(Guid.Parse(userId));
    }
}