using FileApiService.Application.Contracts;
using OnlineFileStorage.Grpc.Shared.Storage; 
using Grpc.Core;
namespace FileApiService.Infrastructure.Grpc;

public class LinkProvider : ILinkProvider
{
    private readonly StorageService.StorageServiceClient _client;
    public LinkProvider(StorageService.StorageServiceClient client)
    {
        _client = client;
    }
    public async Task<string> GetUploadLinkAsync(Guid fileId, CancellationToken ct)
    {
        var request = new UploadLinkRequest { FileId = fileId.ToString() };
        var response = await _client.GetUploadLinkAsync(request, cancellationToken: ct);
        return response.Url;
    }

    public async Task<string> GetDownloadLinkAsync(Guid fileId, string fileName, CancellationToken ct)
    {
        var request = new DownloadLinkRequest 
        { 
            FileId = fileId.ToString(), 
            FileName = fileName 
        };
        
        var response = await _client.GetDownloadLinkAsync(request, cancellationToken: ct);
        return response.Url;
    }
}