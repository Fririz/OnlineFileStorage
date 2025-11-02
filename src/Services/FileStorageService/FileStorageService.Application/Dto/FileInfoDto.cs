namespace FileStorageService.Application.Dto;

public class FileInfoDto
{
    public Guid FileId { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
}