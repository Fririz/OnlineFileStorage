namespace FileStorageService.Application.Exceptions;

public class FileUploadFailedException : Exception
{
    public FileUploadFailedException(string message) : base(message)
    {
        
    }
    
}