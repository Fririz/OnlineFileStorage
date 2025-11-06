using IdentityService.Domain.Entities;
namespace IdentityService.Application.Contracts;

public interface IUserRepository
{
    public Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    public Task<bool> CheckUserExistenceAsync(string username, CancellationToken cancellationToken = default);
    public Task<User?> AddUserAsync(User user, CancellationToken cancellationToken = default);
}