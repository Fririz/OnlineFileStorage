using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class InvalidTypeOfItemError : Error
{
    public InvalidTypeOfItemError(string message) : base(message)
    {
    }
}