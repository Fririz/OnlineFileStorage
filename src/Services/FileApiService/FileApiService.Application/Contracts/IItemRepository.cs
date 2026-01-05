using FileApiService.Domain.Entities;
namespace FileApiService.Application.Contracts;

public interface IItemRepository : IRepositoryBase<Item>
{
    public Task<IEnumerable<Item?>> GetAllChildrenAsync(Guid itemId);
    public Task<Item?> GetParent(Guid itemId);
    public Task<IEnumerable<Item?>> GetRootItems(Guid userId);
    public Task<IEnumerable<Item?>> GetSharedRootItems(Guid userId);
    public Task<int> DeleteFilesWithPendingExpired();

}