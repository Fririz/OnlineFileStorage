using Contracts.Shared;
using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using FluentResults; 
using FileApiService.Application.Exceptions.FluentResultsErrors; 

namespace FileApiService.Application;

public class FolderService : IFolderService
{
    private readonly ILogger<FolderService> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public FolderService(ILogger<FolderService> logger, 
        IItemRepository itemRepository,
        IPublishEndpoint publishEndpoint,
        IMapper mapper)
    {
        _mapper = mapper;
        _logger = logger;
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<List<ItemResponseDto>>> GetChildrenAsync(Guid folderId)
    {
        var items = await _itemRepository.GetAllChildrenAsync(folderId);
        var mappedItems = _mapper.Map(items);
        
        return Result.Ok(mappedItems);
    }

    public async Task<Result<Guid>> CreateFolder(ItemCreateDto itemCreate, Guid ownerId)
    {
        if (itemCreate.Type != TypeOfItem.Folder)
        {
            return Result.Fail(new InvalidTypeOfItemError("Invalid type of item"));
        }
        var folder = Item.CreateFolder(ownerId, itemCreate.Name, itemCreate.ParentId);
        try
        {
            await _itemRepository.AddAsync(folder);
        }
        catch (ItemAlreadyExistsException e)
        {
            return Result.Fail(new FolderAlreadyExistsError("Folder with the same name already exists"));
        }
        
        return Result.Ok(folder.Id);
    }

    public async Task<Result> DeleteFolderWithAllChildren(Guid folderId, Guid ownerId)
    {
        var folder = await _itemRepository.GetByIdAsync(folderId);
        
        if (folder is null)
        {
            return Result.Fail(new DirectoryNotFoundError($"Folder with id {folderId} not found"));
        }

        if (folder.Type != TypeOfItem.Folder)
        {
            return Result.Fail(new InvalidTypeOfItemError($"Item {folderId} is not a folder"));
        }

        if (folder.OwnerId != ownerId)
        {
            return Result.Fail(new UnauthorizedAccessError("You are not allowed to delete this folder"));
        }

        var allItemsToDelete = new List<Item>();
        
        await FindAllDescendantsAsync(folderId, allItemsToDelete);
        
        allItemsToDelete.Add(folder);
        allItemsToDelete.ForEach(x => x.MarkAsDeleted());

        await _publishEndpoint.Publish(new FilesDeletionRequest
        {
            IdsToDelete = allItemsToDelete.Select(x => x.Id).ToList()
        });

        await _itemRepository.UpdateRangeAsync(allItemsToDelete);

        return Result.Ok();
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