using FileApiService.Application.Dto;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IFolderService
{
    public Task<Result<Guid>> CreateFolder(ItemCreateDto itemCreate, Guid ownerId);
    public Task<Result> DeleteFolderWithAllChildren(Guid folderId, Guid ownerId);
    public Task<Result<List<ItemResponseDto>>> GetChildrenAsync(Guid userId);
}