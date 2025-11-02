using FileApiService.Application.Dto;

namespace FileApiService.Application.Contracts;

public interface IFolderWorker
{
    public Task<Guid> CreateFolder(ItemDto item, Guid ownerId);
}