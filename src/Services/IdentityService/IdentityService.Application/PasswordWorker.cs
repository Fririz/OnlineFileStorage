using IdentityService.Application.Contracts;
using BCrypt.Net;

namespace IdentityService.Application;

public class PasswordWorker : IPasswordWorker
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public bool CheckPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}