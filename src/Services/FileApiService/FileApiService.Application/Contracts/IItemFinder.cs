using FileApiService.Domain.Entities;

namespace FileApiService.Application.Contracts;

public interface IItemFinder
{

    public Task<List<Item>> FindItem(string searchQuery, Guid userId, CancellationToken cancellationToken = default);
}