using FileApiService.Domain.Entities;
namespace FileApiService.Application.Contracts;

public interface IItemRepository : IRepositoryBase<Item>
{
    public IEnumerable<Item?> GetAllChildren(Guid itemId);
    public Item? GetParent(Guid itemId);
    public IEnumerable<Item?> GetRootItems(Guid userId);
    public IEnumerable<Item?> GetSharedRootItems(Guid userId);
    public Task<int> DeleteFilesWithPendingExpired();

}