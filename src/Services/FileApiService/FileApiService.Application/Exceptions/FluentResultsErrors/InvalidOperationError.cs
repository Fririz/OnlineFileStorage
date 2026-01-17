using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class InvalidOperationError : Error
{
    public InvalidOperationError(string message) : base(message)
    {
    }
}