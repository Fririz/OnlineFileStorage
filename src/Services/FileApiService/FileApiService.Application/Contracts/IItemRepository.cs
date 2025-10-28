using FileApiService.Domain.Entities;
namespace FileApiService.Application.Contracts;

public interface IItemRepository
{
    public IEnumerable<Item> GetAllChildren(Guid itemId);
    public Item? GetParent(Guid itemId);
    public IEnumerable<Item?> GetRootItems(Guid userId);
    public IEnumerable<Item?> GetSharedRootItems(Guid userId);
}