using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
namespace FileStorageService.Infrastructure;

public class FileRepository : IFileRepository
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<FileRepository> _logger;
    private readonly string _bucketName;
    public FileRepository(IOptions<MinioOptions> minioOptions, ILogger<FileRepository> logger, MinioClient minioClient)
    {
        _logger = logger;
        _minioClient = minioClient;
        _bucketName = minioOptions.Value.BucketName;
    }
}