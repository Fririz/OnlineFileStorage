using FileStorageService.Application.Contracts;

namespace FileStorageService.Application;

public class TokenManager : ITokenManager
{
    public TokenManager()
    {
        
    }
    //TODO add token logic
    private string testtoken = "testtoken";
    
    public string GetToken()
    {
        return testtoken;
    }

    public bool ValidateToken(string? token)
    {
        if (token != testtoken || string.IsNullOrEmpty(token))
        {
            return false;
        }

        return true;
    }
}