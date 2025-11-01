using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace FileStorageService.Infrastructure;

public class FileRepository : IFileRepository
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<FileRepository> _logger;
    private readonly string _bucketName;


    public FileRepository(
        IMinioClient minioClient,
        IConfiguration configuration,
        ILogger<FileRepository> logger)
    {
        _minioClient = minioClient;
        _logger = logger;

        var minioSection = configuration.GetSection("Minio");
        _bucketName = (minioSection["BucketName"] ?? "files").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(_bucketName))
        {
            throw new InvalidOperationException("MinIO bucket name is not configured.");
        }
    }

    public async Task UploadFileAsync(Stream stream, Guid id)
    {
        try
        {
            await EnsureBucketExistsAsync().ConfigureAwait(false);

            Stream payloadStream;
            long payloadLength;

            if (stream.CanSeek)
            {
                try
                {
                    stream.Position = 0;
                }
                catch { /* ignore, попробуем как есть */ }

                payloadStream = stream;
                payloadLength = stream.Length; // для seekable потоков длина доступна
            }
            else
            {
                // Fallback: буферизуем только если нельзя получить длину
                await using var ms = new MemoryStream();
                await stream.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;

                if (ms.Length == 0)
                {
                    _logger.LogWarning("Attempted to upload empty stream for object {Object} to bucket {Bucket}.", id, _bucketName);
                    throw new InvalidOperationException("Uploaded stream is empty.");
                }

                payloadStream = ms;
                payloadLength = ms.Length;

                // Переприсваиваем, чтобы using сработал после PutObjectAsync
                stream = ms;
            }

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(id.ToString("N"))
                .WithStreamData(payloadStream)
                .WithObjectSize(payloadLength)
                .WithContentType("application/octet-stream");

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            _logger.LogInformation("Uploaded object {Object} to bucket {Bucket}, size {Size} bytes.", id, _bucketName, payloadLength);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading object {Object} to bucket {Bucket}.", id, _bucketName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(Guid objectId)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectId.ToString("N"))
                .WithCallbackStream(s =>
                {
                    s.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

            memoryStream.Position = 0;
            _logger.LogInformation("Downloaded object {Object} from bucket {Bucket}, size {Size} bytes.", objectId, _bucketName, memoryStream.Length);
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading object {Object} from bucket {Bucket}.", objectId, _bucketName);
            throw;
        }
    }
    
    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_bucketName))
        {
            _logger.LogCritical("MinIO bucket name is not configured.");
            throw new InvalidOperationException("MinIO bucket name is not configured.");
        }
        var bucket = _bucketName.Trim().ToLowerInvariant();

        try
        {
            var beArgs = new BucketExistsArgs().WithBucket(bucket);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
            {
                _logger.LogWarning("Bucket not found, creating: {Bucket}", bucket);
                var mbArgs = new MakeBucketArgs().WithBucket(bucket);
                await _minioClient.MakeBucketAsync(mbArgs, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Bucket {Bucket} created successfully.", bucket);
            }
            else
            {
                _logger.LogInformation("Bucket {Bucket} already exists.", bucket);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error creating/ensuring bucket: {Bucket}", bucket);
            throw;
        }
    }
}