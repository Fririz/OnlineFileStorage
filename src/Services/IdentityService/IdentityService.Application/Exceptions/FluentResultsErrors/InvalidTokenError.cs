using FluentResults;

namespace IdentityService.Application.Exceptions.FluentResultsErrors;

public class InvalidTokenError : Error
{
    public InvalidTokenError(string message) : base(message){}
}