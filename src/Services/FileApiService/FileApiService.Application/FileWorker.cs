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

    public async Task<Guid> CreateFile(ItemDto item, Guid ownerId)
    {
        if (item.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Creating a folder in method createFile not allowed");
        }
        var file = Item.CreateFile(ownerId, item.Name, item.ParentId);
        await _itemRepository.AddAsync(file);
        return file.Id;
    }
    public async Task<Guid> DeleteFile(ItemDeleteDto item)
    {
        if (item.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Deleting a folder in method createFile not allowed");
        }

        await _itemRepository.DeleteByIdAsync(item.Id);
        return item.Id;
    }
}