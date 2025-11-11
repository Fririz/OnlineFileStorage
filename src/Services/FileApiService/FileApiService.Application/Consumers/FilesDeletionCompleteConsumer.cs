using Contracts.Shared;
using FileApiService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application.Consumers;

public class FilesDeletionCompleteConsumer : IConsumer<FilesDeletionComplete>
{
    private readonly ILogger<FilesDeletionCompleteConsumer> _logger;
    private readonly IItemRepository _itemRepository;
    public FilesDeletionCompleteConsumer(ILogger<FilesDeletionCompleteConsumer> logger,
        IItemRepository itemRepository)
    {
        _logger = logger;
        _itemRepository = itemRepository;
    }
    //TODO: move consumers and rabbitmq logic to another layer
    public async Task Consume(ConsumeContext<FilesDeletionComplete> context)
    {
        var message = context.Message;
        await _itemRepository.DeleteRangeByIdsAsync(message.DeletedIds);
    }
}