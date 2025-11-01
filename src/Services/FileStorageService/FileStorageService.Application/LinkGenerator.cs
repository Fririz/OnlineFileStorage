using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace FileStorageService.Application;

public class LinkGenerator : ILinkGenerator
{
    private readonly ILogger<LinkGenerator> _logger;
    private readonly ITokenManager _tokenManager;
    public LinkGenerator(ILogger<LinkGenerator> logger,
        ITokenManager tokenManager)
    {
        _tokenManager = tokenManager;
        _logger = logger;
    }
    
    public string GenerateDownloadLink(Guid fileId, string filename)
    {
        var token = _tokenManager.GetToken();
        return $"http://localhost:6002/api/download/{fileId}/{filename}/{token}";
    }
    public string GenerateUploadLink(Guid fileId)
    {
        return $"http://localhost:6002/api/upload/{fileId}";
    }
}