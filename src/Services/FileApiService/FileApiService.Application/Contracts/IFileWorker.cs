using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;

namespace FileApiService.Application.Contracts;

public interface IFileWorker
{//TODO write exceptions and documentation
    public Task<string> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<string> CreateFile(ItemCreateDto itemCreate, Guid ownerId, CancellationToken cancellationToken = default);
    public Task DeleteFile(Guid id, Guid ownerId);
    public Task<List<ItemResponseDto>> GetRootItems(Guid userId);
    public Task<Item?> GetParent(Guid itemId);
}