using Contracts.Shared;
using FileApiService.Application.Contracts;
using Microsoft.Extensions.Logging;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using FileApiService.Application.Dto;
using MassTransit;

namespace FileApiService.Application;

public class FileWorker : IFileWorker
{
    private readonly ILogger<FileWorker> _logger;
    private readonly IItemRepository _itemRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    public FileWorker(ILogger<FileWorker> logger, 
        IItemRepository itemRepository, 
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory,
        IMapper mapper
        )
    {
        _logger = logger;
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
        
    }
    public async Task<List<ItemResponseDto>> GetRootItems(Guid userId)
    {
        var itemsEnum = await _itemRepository.GetRootItems(userId);
        var items = itemsEnum.OfType<Item>().ToList();
        var itemDtos = _mapper.Map(items);
        return itemDtos;
    }
    
    public async Task<string> DownloadFile(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {

        var item = await _itemRepository.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new FileNotFoundException("Item not found"); 
        }
        if (item.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You are not allowed to download this file");
        }
        //TODO add GRPC call to filestorage service ITS TEMPORARY
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"http://filestorageservice:8083/api/link/GetDownloadLink/{item.Id}/{item.Name}/", cancellationToken);
        var link = await response.Content.ReadAsStringAsync(cancellationToken);
        return link;
    }
    public async Task<string> CreateFile(ItemCreateDto itemCreate, Guid userId, CancellationToken cancellationToken = default)
    {
        if (itemCreate.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Creating a folder in method createFile not allowed");
        }
        //TODO add name check if exists
        
        var file = Item.CreateFile(userId, itemCreate.Name, itemCreate.ParentId);
        //TODO add GRPC call to filestorage service
        await _itemRepository.AddAsync(file, cancellationToken);;
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"http://filestorageservice:8083/api/link/GetUploadLink/{file.Id}", cancellationToken);
        var link = await response.Content.ReadAsStringAsync(cancellationToken);
        return link;
    }
    public async Task DeleteFile(Guid itemId, Guid userId)
    {
        var file = await _itemRepository.GetByIdAsync(itemId);
        if (file == null)
        {
            throw new FileNotFoundException("Item not found");  
        }
        if (file.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this file");
        }
        if (file.Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Deleting a folder in method delete file not allowed");
        }
        file.MarkAsDeleted();
        await _itemRepository.UpdateAsync(file);
        await _publishEndpoint.Publish(new FileDeletionRequested()
        {
            FileId = file.Id
        });
    }
}