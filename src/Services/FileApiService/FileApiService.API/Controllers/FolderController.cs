using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FolderController : BaseApiController
{
    private readonly IFolderWorker _folderWorker;

    public FolderController(IFolderWorker folderWorker, IItemFinder itemFinder) : base(itemFinder)
    {
        _folderWorker = folderWorker;
    }

    [HttpPost]
    [Route("createfolder")]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        var result = await _folderWorker.CreateFolder(itemCreate, CurrentUserId);
        return HandleResult(result);
    }

    [HttpGet]
    [Route("getallchildren/{id}/")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetAllChildren(Guid id)
    {
        var result = await _folderWorker.GetChildrenAsync(id);
        return HandleResult(result);
    }

    [HttpDelete]
    [Route("deletefolder/{id}/")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid id)
    {
        var result = await _folderWorker.DeleteFolderWithAllChildren(id, CurrentUserId);
        return HandleResult(result);
    }
}