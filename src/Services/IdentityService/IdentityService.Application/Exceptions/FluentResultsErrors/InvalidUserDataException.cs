using FluentResults;

namespace IdentityService.Application.Exceptions.FluentResultsErrors;

public class InvalidUserDataException : Error
{
    public InvalidUserDataException(string message) : base(message){}
}