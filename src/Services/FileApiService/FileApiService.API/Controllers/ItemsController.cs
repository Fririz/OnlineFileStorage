using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : BaseApiController
{
    private readonly IItemService _itemService;
    public ItemsController(IItemService itemService) : base(itemService)
    {
        _itemService = itemService;
        
    }
    
    [HttpGet("{searchQuery}")]
    public async Task<ActionResult<List<ItemResponseDto>>> SearchItem([FromRoute] string searchQuery, CancellationToken cancellationToken)
    {
        var result = await ItemService.FindItem(searchQuery, CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
    /// <summary>
    /// Get all items from root
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>list of items</returns>
    [HttpGet]
    public async Task<ActionResult<List<ItemResponseDto>>> GetItemsFromRoot()
    {
        var result = await _itemService.GetRootItems(CurrentUserId);
        return HandleResult(result);
    }
    /// <summary>
    /// Get parent of file
    /// </summary>
    /// <param name="fileId">fileId</param>
    /// <returns></returns>
    [HttpGet("{fileId:guid}/parent")]
    public async Task<ActionResult<Item>> GetParent(Guid fileId)
    {
        var parent = await _itemService.GetParent(fileId);
        
        if (parent is null)
        {
            return NotFound();
        }

        return Ok(parent);
    }
    
}