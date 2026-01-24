using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class FileAlreadyExistsError : Error
{
    public FileAlreadyExistsError(string message) : base(message)
    {
        
    }
}