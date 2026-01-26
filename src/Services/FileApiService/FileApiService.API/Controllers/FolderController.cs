using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FolderController : BaseApiController
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService, IItemService itemService) : base(itemService)
    {
        _folderService = folderService;
    }

    [HttpPost]
    [Route("createfolder")]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        var result = await _folderService.CreateFolder(itemCreate, CurrentUserId);
        return HandleResult(result);
    }

    [HttpGet]
    [Route("getallchildren/{id}/")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetAllChildren(Guid id)
    {
        var result = await _folderService.GetChildrenAsync(id);
        return HandleResult(result);
    }

    [HttpDelete]
    [Route("deletefolder/{id}/")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid id)
    {
        var result = await _folderService.DeleteFolderWithAllChildren(id, CurrentUserId);
        return HandleResult(result);
    }
}