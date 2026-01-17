using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class UnauthorizedAccessError : Error
{
    public UnauthorizedAccessError(string message) : base(message){}
}