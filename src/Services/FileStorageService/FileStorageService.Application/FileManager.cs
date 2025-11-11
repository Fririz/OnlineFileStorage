using Contracts.Shared;
using FileStorageService.Application.Contracts;
using FileStorageService.Application.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Application;

public class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly ITokenManager _tokenManager;
    private readonly IPublishEndpoint _publishEndpoint;
    public FileManager(ILogger<FileManager> logger, 
        IFileRepository fileRepository,
        ITokenManager tokenManager,
        IPublishEndpoint publishEndpoint
        )
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _tokenManager = tokenManager;
        _publishEndpoint = publishEndpoint;
    }

    public async Task UploadFileCaseAsync(Stream stream, Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _fileRepository.UploadFileAsync(stream, id, cancellationToken);

            var fileInfo = await _fileRepository.GetInfoAboutFile(id, cancellationToken);

            await _publishEndpoint.Publish(new FileUploadComplete()
            {
                FileId = fileInfo.FileId,
                FileSize = fileInfo.FileSize,
                MimeType = fileInfo.MimeType,
            }, cancellationToken); 
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) 
        {
            await _publishEndpoint.Publish(new FileUploadFailed()
            {
                FileId = id
            }, CancellationToken.None); 

            throw new FileUploadFailedException($"File upload failed for id {id}.");
        }
    }

    public async Task<Stream> DownloadFileCaseAsync(Guid objectId, string? token, CancellationToken cancellationToken = default)
    {
        if (_tokenManager.ValidateToken(token) == false)
        {
            throw new UnauthorizedAccessException();
        }
        var stream = await _fileRepository.DownloadFileAsync(objectId, cancellationToken);
        if (stream == null)
        {
            throw new FileNotFoundException();
        }
        return stream;
    }
    public async Task DeleteFileCaseAsync(Guid objectId)
    {
        await _fileRepository.DeleteFileAsync(objectId);
        await _publishEndpoint.Publish(new FileDeletionComplete
        {
            FileId = objectId
        });
    }

    public async Task DelesFilesCaseAsync(IEnumerable<Guid> idsToDelete)
    {
        var deletedIds = idsToDelete.ToArray();
        await _fileRepository.DeleteFilesAsync(deletedIds);
        await _publishEndpoint.Publish(new FilesDeletionComplete
        {
            DeletedIds = deletedIds
        });
    }
    
}