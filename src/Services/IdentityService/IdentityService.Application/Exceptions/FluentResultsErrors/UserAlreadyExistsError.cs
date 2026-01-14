using FluentResults;

namespace IdentityService.Application.Exceptions.FluentResultsErrors;

public class UserAlreadyExistsError : Error
{
    public UserAlreadyExistsError(string message) : base(message)
    {
    }
}