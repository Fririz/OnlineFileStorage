using FileApiService.Application.Dto;

namespace FileApiService.Application.Contracts;

public interface IFolderWorker
{
    public Task<Guid> CreateFolder(ItemDto item, Guid ownerId);
    /// <exception cref="InvalidOperationException">
    /// Creating a file in method createFolder not allowed
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">You are not allowed to delete this folder</exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public Task DeleteFolderWithAllChildren(Guid folderId, Guid ownerId);
}