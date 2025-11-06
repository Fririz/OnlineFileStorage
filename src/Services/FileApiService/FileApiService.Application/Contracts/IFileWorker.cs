using FileApiService.Application.Dto;

namespace FileApiService.Application.Contracts;

public interface IFileWorker
{
    public Task<string> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<string> CreateFile(ItemDto item, Guid ownerId, CancellationToken cancellationToken = default);
    public Task DeleteFile(Guid id, Guid ownerId);
}