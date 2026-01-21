using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class InvalidParentError : Error
{
    public InvalidParentError(string message) : base(message)
    {
    }
}