using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _userContext.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _userContext.Users.AddAsync(user, cancellationToken);
        await _userContext.SaveChangesAsync(cancellationToken);
        return user;
    }
    public async Task<bool> CheckUserExistenceAsync(string username, CancellationToken cancellationToken = default)
    {
        if (await _userContext.Users.AnyAsync(u => u.Username == username, cancellationToken))
        {
            return true;
        }
        return false;
    }
}