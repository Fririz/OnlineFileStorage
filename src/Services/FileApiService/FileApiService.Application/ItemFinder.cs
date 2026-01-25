using FileApiService.Application.Contracts;
using FileApiService.Domain.Entities;

namespace FileApiService.Application;

public class ItemFinder
{
    private readonly IItemRepository _itemRepository;

    public ItemFinder(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<Item>> FindItem(string searchQuery, Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _itemRepository.SearchItemsAsync(searchQuery, userId, cancellationToken);
        throw new NotImplementedException();
    }
}