using Contracts.Shared;
using FileApiService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileApiService.Infrastructure.Consumers;

public class FileDeletionCompleteConsumer : IConsumer<FileDeletionComplete>
{
        private readonly ILogger<FileDeletionCompleteConsumer> _logger;
        private readonly IItemRepository _itemRepository;
        public FileDeletionCompleteConsumer(ILogger<FileDeletionCompleteConsumer> logger,
            IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }
        public async Task Consume(ConsumeContext<FileDeletionComplete> context)
        {
            var message = context.Message;
            var item = await _itemRepository.GetByIdAsync(message.FileId);
            if (item is not null)
                await _itemRepository.DeleteAsync(item);
        }
}