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
    public async Task UploadFileCaseAsync(Stream stream, Guid id)
    {
        try
        {
            await _fileRepository.UploadFileAsync(stream, id);
            var fileInfo = await _fileRepository.GetInfoAboutFile(id);
            await _publishEndpoint.Publish(new FileUploadComplete()
            {
                FileId = fileInfo.FileId,
                FileSize = fileInfo.FileSize,
                MimeType = fileInfo.MimeType,
            });
        }
        catch
        {
            await _publishEndpoint.Publish(new FileUploadFailed()
            {
                FileId = id
            });
            throw new FileUploadFailedException("File upload failed.");
        }

    }
    public async Task<Stream> DownloadFileCaseAsync(Guid objectId, string? token)
    {
        if (_tokenManager.ValidateToken(token) == false)
        {
            throw new UnauthorizedAccessException();
        }
        var stream = await _fileRepository.DownloadFileAsync(objectId);
        if (stream == null)
        {
            throw new FileNotFoundException();
        }
        return stream;
    }
}