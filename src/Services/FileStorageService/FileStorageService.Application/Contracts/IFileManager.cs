using Contracts.Shared;

namespace FileStorageService.Application.Contracts;

public interface IFileManager
{
    public Task UploadFileCaseAsync(Stream stream, Guid id, CancellationToken cancellationToken = default);
    public Task<Stream> DownloadFileCaseAsync(Guid objectId, string? token, CancellationToken cancellationToken = default);
    public Task DeleteFileCaseAsync(Guid objectId);
}