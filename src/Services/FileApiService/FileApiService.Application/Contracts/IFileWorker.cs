using FileApiService.Application.Dto;

namespace FileApiService.Application.Contracts;

public interface IFileWorker
{
    public Task<string> DownloadFile(Guid id);
    public Task<string> CreateFile(ItemDto item, Guid ownerId);
}