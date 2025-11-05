using Contracts.Shared;
using FileStorageService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Application.Consumers;

public class FileDeletionRequestConsumer : IConsumer<FileDeletionRequested>
{
    private readonly ILogger<FileDeletionRequestConsumer> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public FileDeletionRequestConsumer(ILogger<FileDeletionRequestConsumer> logger,
        IFileRepository fileRepository,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<FileDeletionRequested> context)
    {
        var message = context.Message;
        await _fileRepository.DeleteFileAsync(message.FileId);
        _logger.LogInformation("File {FileId} deleted and bus message sended", message.FileId);
        await _publishEndpoint.Publish(new FileDeletionComplete
        {
            FileId = message.FileId,
        });

    }
}