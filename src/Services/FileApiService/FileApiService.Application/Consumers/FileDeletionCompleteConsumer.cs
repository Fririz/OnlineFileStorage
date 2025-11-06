using Contracts.Shared;
using FileApiService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application.Consumers;

public class FileDeletionCompleteConsumer : IConsumer<FileDeletionComplete>
{
        private readonly ILogger<FileUploadCompletedConsumer> _logger;
        private readonly IItemRepository _itemRepository;
        public FileDeletionCompleteConsumer(ILogger<FileUploadCompletedConsumer> logger,
            IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }
        //TODO: move consumers and rabbitmq logic to another layer
        public async Task Consume(ConsumeContext<FileDeletionComplete> context)
        {
            var message = context.Message;
            await _itemRepository.DeleteByIdAsync(message.FileId);
        }
}