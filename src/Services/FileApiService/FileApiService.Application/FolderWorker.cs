using Contracts.Shared;
using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application;

public class FolderWorker : IFolderWorker
{
    private readonly ILogger<FolderWorker> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public FolderWorker(ILogger<FolderWorker> logger, 
        IItemRepository itemRepository,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> CreateFolder(ItemDto item, Guid ownerId)
    {
        if (item.Type != TypeOfItem.Folder)
        {
            throw new InvalidOperationException("Creating a file in method createFolder not allowed");
        }
        var folder = Item.CreateFolder(ownerId, item.Name, item.ParentId);
        await _itemRepository.AddAsync(folder);
        return folder.Id;
    }

    public async Task DeleteFolderWithAllChildren(Guid folderId, Guid ownerId)
    {
        
        var folder = await _itemRepository.GetByIdAsync(folderId);
        if (folder is null)
        {
            throw new DirectoryNotFoundException();
        }

        if (folder.Type != TypeOfItem.Folder)
        {
            
            throw new InvalidOperationException("Creating a file in method createFolder not allowed");
        }

        if (folder.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this folder");
        }
        var allItemsToDelete = new List<Item>();
        await FindAllDescendantsAsync(folderId, allItemsToDelete);
        allItemsToDelete.Add(folder);
        await _itemRepository.DeleteAsync(allItemsToDelete);
        await _publishEndpoint.Publish(new FilesDeletionRequest
        {
            IdsToDelete = allItemsToDelete.Select(x => x.Id).ToList()
        });
    }
    private async Task FindAllDescendantsAsync(Guid parentId, List<Item> allItems)
    {
        var children = await _itemRepository.GetAllChildrenAsync(parentId);
        
        var childrenList = children.ToList(); 

        if (!childrenList.Any())
        {
            return; 
        }

        allItems.AddRange(childrenList.OfType<Item>());

        foreach (var child in childrenList)
        {
            if (child.Type == TypeOfItem.Folder)
            {
                await FindAllDescendantsAsync(child.Id, allItems);
            }
        }
    }
}