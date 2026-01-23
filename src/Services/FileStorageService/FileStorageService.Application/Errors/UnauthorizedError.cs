using FluentResults;

namespace FileStorageService.Application.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError(string message = "Invalid or missing token.") 
        : base(message) 
    {
        Metadata.Add("ErrorCode", "UNAUTHORIZED");
    }
}