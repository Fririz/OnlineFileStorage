using FluentResults;

namespace FileStorageService.Application.Errors;

public class StorageError : Error
{
    public StorageError(string message) : base(message)
    {
        Metadata.Add("ErrorCode", "STORAGE_ERROR");
    }
}