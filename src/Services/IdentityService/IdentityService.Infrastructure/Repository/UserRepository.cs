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

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> AddUserAsync(User user)
    {
        await _userContext.Users.AddAsync(user);
        await _userContext.SaveChangesAsync();
        return user;
    }
    public async Task<bool> CheckUserExistenceAsync(string username)
    {
        if (await _userContext.Users.AnyAsync(u => u.Username == username))
        {
            return true;
        }
        return false;
    }
}