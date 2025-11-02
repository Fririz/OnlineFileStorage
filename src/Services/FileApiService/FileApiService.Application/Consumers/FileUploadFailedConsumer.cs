using Contracts.Shared;
using FileApiService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application.Consumers;

public class FileUploadFailedConsumer : IConsumer<FileUploadFailed>
{
    private readonly ILogger<FileUploadCompletedConsumer> _logger;
    private readonly IItemRepository _itemRepository;
    public FileUploadFailedConsumer(ILogger<FileUploadCompletedConsumer> logger,
        IItemRepository itemRepository)
    {
        _logger = logger;
        _itemRepository = itemRepository;
    }
    
    public async Task Consume(ConsumeContext<FileUploadFailed> context)
    {
        var message = context.Message;
        var item = await _itemRepository.GetByIdAsync(message.FileId);
        if (item == null)
            return;
        item.MarkUploadAsFailed();
        await _itemRepository.UpdateAsync(item);
    }
}