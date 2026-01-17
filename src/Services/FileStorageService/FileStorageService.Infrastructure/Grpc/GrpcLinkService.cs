using FileStorageService.Application.Contracts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OnlineFileStorage.Grpc.Shared.Storage;

namespace FileStorageService.Infrastructure.Grpc;

public class GrpcLinkService : StorageService.StorageServiceBase
{
    private readonly ILinkGenerator _linkGenerator;
    private readonly ILogger<GrpcLinkService> _logger;

    public GrpcLinkService(ILinkGenerator linkGenerator, ILogger<GrpcLinkService> logger)
    {
        _linkGenerator = linkGenerator;
        _logger = logger;
    }
    
    public override Task<LinkResponse> GetDownloadLink(DownloadLinkRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.FileId, out var guid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Guid format"));
        }
        
        var url = _linkGenerator.GenerateDownloadLink(guid, request.FileName);

        return Task.FromResult(new LinkResponse { Url = url });
    }

    // ДОБАВЛЕНО: override
    public override Task<LinkResponse> GetUploadLink(UploadLinkRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.FileId, out var guid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Guid format"));
        }

        var url = _linkGenerator.GenerateUploadLink(guid);

        return Task.FromResult(new LinkResponse { Url = url });
    }
}