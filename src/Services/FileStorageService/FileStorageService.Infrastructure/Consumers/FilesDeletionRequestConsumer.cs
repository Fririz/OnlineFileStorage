using Contracts.Shared;
using FileStorageService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Infrastructure.Consumers;

public class FilesDeletionRequestConsumer : IConsumer<FilesDeletionRequest>
{
    private readonly IFileManager _fileManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<FilesDeletionRequestConsumer> _logger;
    public FilesDeletionRequestConsumer(ILogger<FilesDeletionRequestConsumer> logger,
        IFileManager fileManager,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _fileManager = fileManager;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<FilesDeletionRequest> context)
    {
        await _fileManager.DeleteFilesCaseAsync(context.Message.IdsToDelete);
    }
}