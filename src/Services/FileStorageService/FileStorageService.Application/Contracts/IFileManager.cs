using Contracts.Shared;
using FileStorageService.Application.Dto;
using FluentResults;

namespace FileStorageService.Application.Contracts;

public interface IFileManager
{
    public Task<Result> UploadFileCaseAsync(Stream stream, Guid id, CancellationToken cancellationToken = default);
    public Task<Result<Stream>> DownloadFileCaseAsync(Guid objectId, string? token, CancellationToken cancellationToken = default);
    public Task<Result> DeleteFileCaseAsync(Guid objectId);
    public Task<Result> DeleteFilesCaseAsync(IEnumerable<Guid> idsToDelete);
}