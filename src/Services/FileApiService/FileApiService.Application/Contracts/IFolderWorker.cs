using FileApiService.Application.Dto;

namespace FileApiService.Application.Contracts;

public interface IFolderWorker
{//TODO write exceptions and documentation
    public Task<Guid> CreateFolder(ItemCreateDto itemCreate, Guid ownerId);
    /// <exception cref="InvalidOperationException">
    /// Creating a file in method createFolder not allowed
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">You are not allowed to delete this folder</exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public Task DeleteFolderWithAllChildren(Guid folderId, Guid ownerId);
    public Task<IEnumerable<ItemResponseDto>> GetChildrenAsync(Guid userId);
}