using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoldersController : BaseApiController
{
    private readonly IFolderService _folderService;

    public FoldersController(IFolderService folderService, IItemService itemService) : base(itemService)
    {
        _folderService = folderService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        var result = await _folderService.CreateFolder(itemCreate, CurrentUserId);
        return HandleResult(result);
    }

    [HttpGet]
    [Route("{folderId}/items")]
    public async Task<ActionResult<List<ItemResponseDto>>> GetAllChildren(Guid folderId)
    {
        var result = await _folderService.GetChildrenAsync(folderId);
        return HandleResult(result);
    }

    [HttpDelete]
    [Route("{folderId}")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid folderId)
    {
        var result = await _folderService.DeleteFolderWithAllChildren(folderId, CurrentUserId);
        return HandleResult(result);
    }
}