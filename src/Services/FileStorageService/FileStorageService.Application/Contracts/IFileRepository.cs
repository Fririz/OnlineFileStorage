namespace FileStorageService.Application.Contracts;

public interface IFileRepository
{
    Task UploadFileAsync(Stream stream, Guid id);
    public Task<Stream> DownloadFileAsync(Guid objectId);
}