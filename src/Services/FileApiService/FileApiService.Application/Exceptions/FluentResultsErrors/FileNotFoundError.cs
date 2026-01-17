using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class FileNotFoundError : Error
{
    public FileNotFoundError(string message) : base(message){}
}