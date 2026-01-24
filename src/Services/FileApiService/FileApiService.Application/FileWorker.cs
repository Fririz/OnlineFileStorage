using Contracts.Shared;
using FileApiService.Application.Contracts;
using Microsoft.Extensions.Logging;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using MassTransit;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FileApiService.Application;

public class FileWorker : IFileWorker
{
    private readonly ILogger<FileWorker> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILinkProvider _linkProvider;
    private readonly IMapper _mapper;
    public FileWorker(ILogger<FileWorker> logger, 
        IItemRepository itemRepository, 
        IPublishEndpoint publishEndpoint,
        IMapper mapper,
        ILinkProvider linkProvider
        )
    {
        _logger = logger;
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
        _linkProvider = linkProvider;
        _mapper = mapper;
        
    }

    public  Task<Item?> GetParent(Guid itemId)
    {
        var parent = _itemRepository.GetParent(itemId);
        return parent;
    }
    public async Task<Result<List<ItemResponseDto>>> GetRootItems(Guid userId)
    {
        var itemsEnum = await _itemRepository.GetRootItems(userId);
        var items = itemsEnum.OfType<Item>().ToList();
        var itemDtos = _mapper.Map(items);
        return Result.Ok(itemDtos);
    }
    
    public async Task<Result<string>> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return Result.Fail(new FileNotFoundError("Item not found"));
        }
        if (item.OwnerId != ownerId)
        {
            return Result.Fail(new UnauthorizedAccessError("You are not allowed to download this file"));
        }
        var link = await _linkProvider.GetDownloadLinkAsync(item.Id, item.Name, cancellationToken);
        
        return Result.Ok(link);
    }
    
    public async Task<Result<string>> CreateFile(ItemCreateDto itemCreate, Guid userId, CancellationToken cancellationToken = default)
    {
        if (itemCreate.Type != TypeOfItem.File)
        {
            return Result.Fail(new InvalidTypeOfItemError("Invalid type of item"));
        }
        if (itemCreate.ParentId != null)// if not from root
        {
            var parent = await _itemRepository.GetByIdAsync((Guid)itemCreate.ParentId, cancellationToken);
            if (parent == null)
                return Result.Fail(new InvalidParentError("Invalid parent id"));
        }//TODO add unit test
        
        
        var file = Item.CreateFile(userId, itemCreate.Name, itemCreate.ParentId);
        try
        {
            await _itemRepository.AddAsync(file, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            return Result.Fail(new FileAlreadyExistsError("File with the same name already exists"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file"); 

            throw;
        }
        try
        {
            _logger.LogInformation($"Created file {file.Id}");
            
            var link = await _linkProvider.GetUploadLinkAsync(file.Id, cancellationToken);
            
            _logger.LogInformation($"Link for uploading = {link}");
            return Result.Ok(link);
        }
        catch
        {
            await _itemRepository.DeleteAsync(file, cancellationToken);
            throw;
        }
    }
    public async Task<Result> DeleteFile(Guid itemId, Guid userId)
    {
        var file = await _itemRepository.GetByIdAsync(itemId);
        if (file == null)
        {
            return Result.Fail(new FileNotFoundError("Item not found"));
        }
        if (file.OwnerId != userId)
        {
            return Result.Fail(new UnauthorizedAccessError("You are not allowed to delete this file"));
        }
        if (file.Type != TypeOfItem.File)
        {
            return Result.Fail(new InvalidTypeOfItemError("Deleting a folder in method delete file not allowed"));
        }
        file.MarkAsDeleted();
        await _publishEndpoint.Publish(new FileDeletionRequested()
        {
            FileId = file.Id
        });
        await _itemRepository.UpdateAsync(file);
        return Result.Ok();
    }
    
    
}