using IdentityService.Domain.Common;
using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class User : EntityBase
{
    private User()
    {
        
    }
    public User(string username, string email, string passwordHash, Role roleOfUser)
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        RoleOfUser = roleOfUser;
    }
    public string Username { get; private set; }
    public string Email { get;  private set; } 
    public string PasswordHash { get;  private set; }
    public Role RoleOfUser { get; private set; }
    
    public void ChangeUsername(string newUsername)
    {
        Username = newUsername;
    }

    public void ChangeEmail(string newEmail)
    {
        Email = newEmail;
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