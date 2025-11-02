using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Application;

public class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly ITokenManager _tokenManager;
    public FileManager(ILogger<FileManager> logger, 
        IFileRepository fileRepository,
        ITokenManager tokenManager)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _tokenManager = tokenManager;
    }
    public async Task UploadFileCaseAsync(Stream stream, Guid id)
    {

        await _fileRepository.UploadFileAsync(stream, id);
        
        //TODO add more logic and rabbiq mq when upload is done
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