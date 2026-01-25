using FileApiService.Application.Contracts;
using FileApiService.Application.ItemExtensions;
using FileApiService.Domain.Entities;

namespace FileApiService.Application;

public class ItemFinder : IItemFinder
{
    private readonly IItemRepository _itemRepository;
    private readonly int _displayLimit = 5;
    public ItemFinder(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<Item>> FindItem(string searchQuery, Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _itemRepository.SearchItemsAsync(searchQuery, userId, cancellationToken);
        var sortedItems = GetItemsSortedByMimeType(items);
        return sortedItems;
    }
    
    private List<Item> GetItemsSortedByMimeType(List<Item> items)
    {
        return items.OrderByDescending(i => MimeTypeSearchWeight.Get(i.MimeType)).Take(_displayLimit).ToList();
    }
}