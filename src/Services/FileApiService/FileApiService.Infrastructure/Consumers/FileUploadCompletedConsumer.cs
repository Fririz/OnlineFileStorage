using Contracts.Shared;
using FileApiService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Infrastructure.Consumers;

public class FileUploadCompletedConsumer : IConsumer<FileUploadComplete>
{
    private readonly ILogger<FileUploadCompletedConsumer> _logger;
    private readonly IItemRepository _itemRepository;
    public FileUploadCompletedConsumer(ILogger<FileUploadCompletedConsumer> logger,
        IItemRepository itemRepository)
    {
        _logger = logger;
        _itemRepository = itemRepository;
    }
    public async Task Consume(ConsumeContext<FileUploadComplete> context)
    {
        var message = context.Message;
        var item = await _itemRepository.GetByIdAsync(message.FileId);
        if (item == null)
            return;
        item.CompleteUpload(message.FileSize, message.MimeType ?? "application/octet-stream");
        await _itemRepository.UpdateAsync(item);
    }
}