using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IFileService
{
    public Task<Result<string>> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<Result<string>> CreateFile(ItemCreateDto itemCreate, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<Result> DeleteFile(Guid itemId, Guid userId);
}