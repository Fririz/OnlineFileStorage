using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : BaseApiController
{
    public SearchController(IItemFinder itemFinder, ILogger<SearchController> logger) : base(itemFinder)
    {
    }
    
    [HttpGet("{searchQuery}")]
    public async Task<ActionResult<List<ItemResponseDto>>> SearchItem([FromRoute] string searchQuery, CancellationToken cancellationToken)
    {
        var result = await _itemFinder.FindItem(searchQuery, CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
}