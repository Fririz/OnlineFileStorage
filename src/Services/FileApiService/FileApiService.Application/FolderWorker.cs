using FileApiService.Application.Contracts;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application;

public class FolderWorker : IFolderWorker
{
    private readonly ILogger<FileWorker> _logger;
    private readonly IItemRepository _itemRepository;
    public FolderWorker(ILogger<FileWorker> logger, IItemRepository itemRepository)
    {
        _logger = logger;
        _itemRepository = itemRepository;
    }

    public async Task<Guid> CreateFolder(Item item)
    {
        if (item.Type != TypeOfItem.Folder)
        {
            throw new InvalidOperationException("Creating a file in method createFolder not allowed");
        }
        var folder = Item.CreateFolder(item.OwnerId, item.Name, item.ParentId);
        await _itemRepository.AddAsync(folder);
        return folder.Id;
    }
}