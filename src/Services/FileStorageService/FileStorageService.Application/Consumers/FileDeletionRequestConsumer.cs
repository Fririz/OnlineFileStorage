using Contracts.Shared;
using FileStorageService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Application.Consumers;

public class FileDeletionRequestConsumer : IConsumer<FileDeletionRequested>
{
    private readonly ILogger<FileDeletionRequestConsumer> _logger;
    private readonly IFileManager _fileManager;
    private readonly IPublishEndpoint _publishEndpoint;
    public FileDeletionRequestConsumer(ILogger<FileDeletionRequestConsumer> logger,
        IFileManager fileManager,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _fileManager = fileManager;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<FileDeletionRequested> context)
    {  //TODO: move consumers and rabbitmq logic to another layer
        await _fileManager.DeleteFileCaseAsync(context.Message.FileId);
        _logger.LogInformation("File {FileId} deleted and bus message sended", context.Message.FileId);

    }
}