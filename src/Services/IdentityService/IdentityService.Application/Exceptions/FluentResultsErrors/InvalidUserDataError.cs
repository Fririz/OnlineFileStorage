using FluentResults;

namespace IdentityService.Application.Exceptions.FluentResultsErrors;

public class InvalidUserDataError : Error
{
    public InvalidUserDataError(string message) : base(message){}
}