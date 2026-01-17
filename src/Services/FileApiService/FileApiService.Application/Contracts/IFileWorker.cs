using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IFileWorker
{
    public Task<Result<string>> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<Result<string>> CreateFile(ItemCreateDto itemCreate, Guid ownerId, CancellationToken cancellationToken = default);
    public Task<Result> DeleteFile(Guid itemId, Guid userId);
    public Task<Result<List<ItemResponseDto>>> GetRootItems(Guid userId);
    public Task<Item?> GetParent(Guid itemId);
}