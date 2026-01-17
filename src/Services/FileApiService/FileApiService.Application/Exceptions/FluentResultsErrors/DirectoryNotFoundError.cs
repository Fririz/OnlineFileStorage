using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class DirectoryNotFoundError : Error
{
    public DirectoryNotFoundError(string message) : base(message)
    {
        
    }
}