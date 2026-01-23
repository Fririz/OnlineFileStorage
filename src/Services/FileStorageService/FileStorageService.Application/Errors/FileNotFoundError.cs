using FluentResults;

namespace FileStorageService.Application.Errors;

public class FileNotFoundError : Error
{
    public FileNotFoundError(Guid fileId) 
        : base($"File with id {fileId} was not found.") 
    {
        Metadata.Add("ErrorCode", "FILE_NOT_FOUND");
    }
}