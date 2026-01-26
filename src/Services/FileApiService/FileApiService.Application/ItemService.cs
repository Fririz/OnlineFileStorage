using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.ItemExtensions;
using FileApiService.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemService> _logger;
    private const int DisplayLimit = 5;
    public ItemService(IItemRepository itemRepository, IMapper mapper,  ILogger<ItemService> logger)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<List<ItemResponseDto>>> GetRootItems(Guid userId)
    {
        var itemsEnum = await _itemRepository.GetRootItems(userId);
        var items = itemsEnum.OfType<Item>().ToList();
        var itemDtos = _mapper.Map(items);
        return Result.Ok(itemDtos);
    }
    
    public  Task<Item?> GetParent(Guid itemId)
    {
        var parent = _itemRepository.GetParent(itemId);
        return parent;
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
        return items.OrderByDescending(i => MimeTypeSearchWeight.Get(i.MimeType)).Take(DisplayLimit).ToList();
    }
}