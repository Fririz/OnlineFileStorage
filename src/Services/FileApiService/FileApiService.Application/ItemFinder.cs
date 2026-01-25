using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.ItemExtensions;
using FileApiService.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application;

public class ItemFinder : IItemFinder
{
    private readonly IItemRepository _itemRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemFinder> _logger;
    private readonly int _displayLimit = 5;
    public ItemFinder(IItemRepository itemRepository, IMapper mapper,  ILogger<ItemFinder> logger)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ItemResponseDto>>> FindItem(string searchQuery, Guid userId, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrEmpty(searchQuery))
            return Result.Fail("Search query is empty");
        if(userId == Guid.Empty)
            return Result.Fail("User id is empty");
        var items = await _itemRepository.SearchItemsAsync(searchQuery, userId, cancellationToken);
        
        var sortedItems = GetItemsSortedByMimeType(items);
        var mappedItems = _mapper.Map(sortedItems);
        return mappedItems;
    }
    
    private List<Item> GetItemsSortedByMimeType(List<Item> items)
    {
        return items.OrderByDescending(i => MimeTypeSearchWeight.Get(i.MimeType)).Take(_displayLimit).ToList();
    }
}