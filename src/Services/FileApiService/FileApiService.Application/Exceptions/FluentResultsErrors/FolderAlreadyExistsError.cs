using FluentResults;

namespace FileApiService.Application.Exceptions.FluentResultsErrors;

public class FolderAlreadyExistsError : Error
{
    public FolderAlreadyExistsError(string message) : base(message){}
}