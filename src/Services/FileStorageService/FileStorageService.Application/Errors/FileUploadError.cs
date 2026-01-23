using FluentResults;

namespace FileStorageService.Application.Errors;

public class FileUploadError : Error
{
    public FileUploadError(string message) : base(message)
    {
        Metadata.Add("ErrorCode", "UPLOAD_FAILED");
    }
}