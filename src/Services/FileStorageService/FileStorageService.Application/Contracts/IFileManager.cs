namespace FileStorageService.Application.Contracts;

public interface IFileManager
{
    public Task UploadFileCaseAsync(Stream stream, Guid id);
    public Task<Stream> DownloadFileCaseAsync(Guid objectId, string? token);
}