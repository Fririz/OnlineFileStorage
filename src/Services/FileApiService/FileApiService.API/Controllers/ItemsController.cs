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
    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
        
    }
    
    //Search + get items from root
    [HttpGet]
    public async Task<ActionResult<List<ItemResponseDto>>> GetItems(
        [FromQuery] string? searchQuery, 
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(searchQuery))
        {
            var result = await _itemService.FindItem(searchQuery, CurrentUserId, cancellationToken);
            return HandleResult(result);
        }

        var rootResult = await _itemService.GetRootItems(CurrentUserId);
        return HandleResult(rootResult);
    }
    /// <summary>
    /// Get parent of file
    /// </summary>
    /// <param name="itemId">fileId</param>
    /// <returns></returns>
    [HttpGet("{itemId:guid}/parent")]
    public async Task<ActionResult<Item>> GetParent(Guid itemId)
    {
        var parent = await _itemService.GetParent(itemId);
        
        if (parent is null)
        {
            return NotFound();
        }

        return Ok(parent);
    }
    
}