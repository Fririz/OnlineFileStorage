using System.ComponentModel.DataAnnotations;
using IdentityService.Domain.Common;
using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class User : EntityBase
{
    [Required]
    [MaxLength(50)]
    public string Username { get; private set; }
    [Required]
    public string PasswordHash { get;  private set; }
    [Required]
    public Role RoleOfUser { get; private set; } = Role.User;
    private User() : base()
    {
        
    }

    public static User CreateUser(string username, string passwordHash, Role roleOfUser)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Username and password cannot be empty.");
        }
        return new User()
        {
            Username = username,
            PasswordHash = passwordHash,
            RoleOfUser = roleOfUser,
            LastModifiedDate = DateTime.UtcNow,
        };
    }
    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password cannot be empty.");       
        }
        PasswordHash = passwordHash;
        LastModifiedDate = DateTime.UtcNow;
    }
    public void UpdateRole(Role roleOfUser)
    {
        RoleOfUser = roleOfUser;
        LastModifiedDate = DateTime.UtcNow;
    }
    public void UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.");
        }
        Username = username;
        LastModifiedDate = DateTime.UtcNow;
    }
}