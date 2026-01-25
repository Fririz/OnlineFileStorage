using FileApiService.Domain.Entities;
namespace FileApiService.Application.Contracts;

public interface IItemRepository : IRepositoryBase<Item>
{
    public Task<IEnumerable<Item?>> GetAllChildrenAsync(Guid itemId, CancellationToken cancellationToken = default);

    public Task<List<Item>> SearchItemsAsync(string searchQuery, Guid userId,
        CancellationToken cancellationToken = default);
    public Task<Item?> GetParent(Guid itemId, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Item?>> GetRootItems(Guid userId, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Item?>> GetSharedRootItems(Guid userId, CancellationToken cancellationToken = default);
    public Task<int> DeleteFilesWithPendingExpired();

}