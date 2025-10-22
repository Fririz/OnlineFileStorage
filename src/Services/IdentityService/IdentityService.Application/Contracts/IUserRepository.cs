using IdentityService.Domain.Entities;
namespace IdentityService.Application.Contracts;

public interface IUserRepository
{
    public Task<User?> GetUserByUsernameAsync(string username);
    public Task<bool> CheckUserExistenceAsync(string username);
    public Task<User?> AddUserAsync(User user);
}