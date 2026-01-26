using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : BaseApiController
{
    public ItemsController(IItemService itemService, ILogger<ItemsController> logger) : base(itemService)
    {
    }
    
    [HttpGet("{searchQuery}")]
    public async Task<ActionResult<List<ItemResponseDto>>> SearchItem([FromRoute] string searchQuery, CancellationToken cancellationToken)
    {
        var result = await ItemService.FindItem(searchQuery, CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
    
    
}