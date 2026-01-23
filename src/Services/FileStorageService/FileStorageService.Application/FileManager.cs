using Contracts.Shared;
using FileSignatures;
using FileStorageService.Application.Contracts;
using FileStorageService.Application.Errors;
using MassTransit;
using Microsoft.Extensions.Logging;
using FluentResults;

namespace FileStorageService.Application;

public class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly ITokenManager _tokenManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IFileFormatInspector _fileFormatInspector;

    public FileManager(ILogger<FileManager> logger, 
        IFileRepository fileRepository,
        ITokenManager tokenManager,
        IPublishEndpoint publishEndpoint,
        IFileFormatInspector fileFormatInspector)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _tokenManager = tokenManager;
        _publishEndpoint = publishEndpoint;
        _fileFormatInspector = fileFormatInspector;
    }

    public async Task<Result> UploadFileCaseAsync(Stream stream,long size, string contentType, Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading file {FileId}, Size: {Size}", id, size);
        bool fileUploaded = false;
        
        try
        {
            await _fileRepository.UploadFileAsync(stream, size, contentType, id, cancellationToken);
            fileUploaded = true;
            
            await _publishEndpoint.Publish(new FileUploadComplete()
            {
                FileId = id,
                FileSize = size,        
                MimeType = contentType, 
            }, cancellationToken);

            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (fileUploaded)
            {
                try
                {
                    await _fileRepository.DeleteFileAsync(id); 
                    await _publishEndpoint.Publish(new FileDeletionComplete { FileId = id }, CancellationToken.None);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogWarning(deleteEx, "Failed to rollback file upload for {FileId}", id);
                }
            }
            
            _logger.LogError(ex, "Error uploading file {FileId}", id);
            await _publishEndpoint.Publish(new FileUploadFailed()
            {
                FileId = id
            }, CancellationToken.None); 
            return Result.Fail(new FileUploadError($"File upload failed for id {id}. Caused by: {ex.Message}"));
        }
    }

    public async Task<Result<Stream>> DownloadFileCaseAsync(Guid objectId, string? token, CancellationToken cancellationToken = default)
    {
        if (_tokenManager.ValidateToken(token) == false)
        {
            return Result.Fail(new UnauthorizedError());
        }

        try 
        {
            var stream = await _fileRepository.DownloadFileAsync(objectId, cancellationToken);
            
            if (stream == null)
            {
                return Result.Fail(new FileNotFoundError(objectId));
            }

            return Result.Ok(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileId}", objectId);
            return Result.Fail(new StorageError("Error accessing file storage."));
        }
    }

    public async Task<Result> DeleteFileCaseAsync(Guid objectId)
    {
        try
        {
            await _fileRepository.DeleteFileAsync(objectId);
            await _publishEndpoint.Publish(new FileDeletionComplete
            {
                FileId = objectId
            });
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileId}", objectId);
            return Result.Fail(new StorageError($"Failed to delete file {objectId}"));
        }
    }

    public async Task<Result> DeleteFilesCaseAsync(IEnumerable<Guid> idsToDelete)
    {
        try
        {
            var deletedIds = idsToDelete.ToArray();
            await _fileRepository.DeleteFilesAsync(deletedIds);
            await _publishEndpoint.Publish(new FilesDeletionComplete
            {
                DeletedIds = deletedIds
            });
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting files batch");
            return Result.Fail(new StorageError("Failed to delete files batch"));
        }
    }
}