namespace FileStorageService.Application.Contracts;

public interface ITokenManager
{
    public string GetToken();
    public bool ValidateToken(string? token);
}