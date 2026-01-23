using FileStorageService.Application.Dto;

namespace FileStorageService.Application.Contracts;

public interface IFileRepository
{
    Task UploadFileAsync(Stream stream, string contentType,Guid id, CancellationToken cancellationToken = default);
    public Task<Stream> DownloadFileAsync(Guid objectId, CancellationToken cancellationToken = default);

    public Task EnsureBucketExistsAsync(CancellationToken cancellationToken = default);
    public Task<FileInfoDto> GetInfoAboutFile(Guid objectId, CancellationToken cancellationToken = default);
    public Task DeleteFileAsync(Guid objectId, CancellationToken cancellationToken = default);
    public Task DeleteFilesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}