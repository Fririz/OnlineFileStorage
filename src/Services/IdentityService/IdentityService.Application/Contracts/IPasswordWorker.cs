namespace IdentityService.Application.Contracts;

public interface IPasswordWorker
{
    public string HashPassword(string password);

    public bool CheckPassword(string password, string hashedPassword);
}