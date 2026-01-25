using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : BaseApiController
{
    public SearchController(IItemFinder itemFinder) : base(itemFinder)
    {
    }
    
    [HttpGet("searchItem")]
    public async Task<ActionResult<List<ItemResponseDto>>> SearchItem([FromQuery] string searchQuery, CancellationToken cancellationToken)
    {
        var result = await _itemFinder.FindItem(searchQuery, CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
}