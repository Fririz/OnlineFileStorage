namespace FileApiService.Application.Contracts;

public interface ILinkProvider
{
    Task<string> GetUploadLinkAsync(Guid fileId, CancellationToken ct);
    Task<string> GetDownloadLinkAsync(Guid fileId, string fileName, CancellationToken ct);
}