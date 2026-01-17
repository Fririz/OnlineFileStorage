using FileApiService.Application.Dto;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IFolderWorker
{//TODO write exceptions and documentation
    public Task<Result<Guid>> CreateFolder(ItemCreateDto itemCreate, Guid ownerId);
    /// <exception cref="InvalidOperationException">
    /// Creating a file in method createFolder not allowed
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">You are not allowed to delete this folder</exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public Task<Result> DeleteFolderWithAllChildren(Guid folderId, Guid ownerId);
    public Task<Result<List<ItemResponseDto>>> GetChildrenAsync(Guid userId);
}