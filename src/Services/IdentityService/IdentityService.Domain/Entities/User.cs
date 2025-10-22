using IdentityService.Domain.Common;
using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class User : EntityBase
{
    private User()
    {
        
    }
    public User(string username, string passwordHash, Role roleOfUser)
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        Username = username;
        PasswordHash = passwordHash;
        RoleOfUser = roleOfUser;
    }
    public User(string username, string passwordHash)
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        Username = username;
        PasswordHash = passwordHash;
    }
    public string Username { get; private set; }
    public string PasswordHash { get;  private set; }
    public Role RoleOfUser { get; private set; } = Role.User;
    
    public void ChangeUsername(string newUsername)
    {
        Username = newUsername;
    }
    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void ChangeRole(Role newRole)
    {
        RoleOfUser = newRole;
    }
}