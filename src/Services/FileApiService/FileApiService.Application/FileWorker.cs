using FileApiService.Application.Contracts;
using Microsoft.Extensions.Logging;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using FileApiService.Application.Dto;
namespace FileApiService.Application;

public class FileWorker : IFileWorker
{
    ILogger<FileWorker> _logger;
    IItemRepository _itemRepository;
    public FileWorker(ILogger<FileWorker> logger, IItemRepository itemRepository)
    {
        _logger = logger;
        _itemRepository = itemRepository;
    }

    public async Task<string> DownloadFile(Guid id)
    {
        //TODO add GRPC call to filestorage service
        var item = await _itemRepository.GetByIdAsync(id);
        HttpClient client = new();
        var response = await client.GetAsync($"http://filestorageservice:8083/api/link/GetDownloadLink/{item.Id}/{item.Name}/");
        var link = await response.Content.ReadAsStringAsync();
        return link;
    }
    public async Task<string> CreateFile(ItemDto item, Guid ownerId)
    {
        if (item.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Creating a folder in method createFile not allowed");
        }
        //TODO add name check if exists
        var file = Item.CreateFile(ownerId, item.Name, item.ParentId);
        //TODO add GRPC call to filestorage service
        await _itemRepository.AddAsync(file);
        HttpClient client = new();
        var response = await client.GetAsync($"http://filestorageservice:8083/api/link/GetUploadLink/{file.Id}");
        var link = await response.Content.ReadAsStringAsync();
        return link;
    }
    public async Task<Guid> DeleteFile(ItemDeleteDto item)
    {
        if (item.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Deleting a folder in method createFile not allowed");
        }
        //TODO add softly delete file
        // await _itemRepository.DeleteByIdAsync(item.Id);
        return item.Id;
    }
}