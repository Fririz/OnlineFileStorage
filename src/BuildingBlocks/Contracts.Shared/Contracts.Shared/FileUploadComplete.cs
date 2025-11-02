namespace Contracts.Shared;

public record FileUploadComplete
{
    public Guid FileId { get; init; }
    public long FileSize { get; init; }
    public string? MimeType { get; init; }
}