using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;

namespace IdentityService.UnitTests;

public class UserTest
{

    [Fact]
    public void CreateUser_ShouldCreateUserCorrectly()
    {
        var username = "testuser";
        var passwordHash = "testPasswordHash";
        var roleOfUser = Role.User;
        
        var user = User.CreateUser(username, passwordHash, roleOfUser);
        
        Assert.Equal(username, user.Username);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(roleOfUser, user.RoleOfUser);
        Assert.NotEqual(default, user.CreatedDate); 
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateUser_ShouldThrowArgumentException_WhenUserNameIsNullOrEmpty(string username)
    {
        var passwordHash = "testPasswordHash";
        var roleOfUser = Role.User;
        Assert.Throws<ArgumentException>(() => User.CreateUser(username, passwordHash, roleOfUser));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateUser_ShouldThrowArgumentException_WhenPasswordHashIsNullOrEmpty(string passwordHash)
    {
        var username = "testUserName";
        var roleOfUser = Role.User;
        Assert.Throws<ArgumentException>(() => User.CreateUser(username, passwordHash, roleOfUser));
    }


    [Fact]
    public void UpdatePassword_ShouldUpdatePasswordAndLastModifiedDate()
    {
        var user = User.CreateUser("testuser", "oldHash", Role.User);
        var newPasswordHash = "newSecretHash";
        var oldModifiedDate = user.LastModifiedDate;

        Thread.Sleep(20); 
        user.UpdatePassword(newPasswordHash);

        Assert.Equal(newPasswordHash, user.PasswordHash);
        Assert.True(user.LastModifiedDate > oldModifiedDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdatePassword_ShouldThrowArgumentException_WhenPasswordHashIsNullOrEmpty(string newPasswordHash)
    {

        var user = User.CreateUser("testuser", "validHash", Role.User);

        Assert.Throws<ArgumentException>(() => user.UpdatePassword(newPasswordHash));
    }

    [Fact]
    public void UpdateUsername_ShouldUpdateUsernameAndLastModifiedDate()
    {
        var user = User.CreateUser("oldName", "hash", Role.User);
        var newUsername = "newName";
        var oldModifiedDate = user.LastModifiedDate;

        Thread.Sleep(20);
        user.UpdateUsername(newUsername);

        Assert.Equal(newUsername, user.Username);
        Assert.True(user.LastModifiedDate > oldModifiedDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateUsername_ShouldThrowArgumentException_WhenUsernameIsNullOrEmpty(string newUsername)
    {
        var user = User.CreateUser("validName", "hash", Role.User);

        Assert.Throws<ArgumentException>(() => user.UpdateUsername(newUsername));
    }

    [Fact]
    public void UpdateRole_ShouldUpdateRoleAndLastModifiedDate()
    {
        var user = User.CreateUser("testuser", "hash", Role.User);
        
        var newRole = Role.Admin; 
        var oldModifiedDate = user.LastModifiedDate;

        Thread.Sleep(20);
        user.UpdateRole(newRole);

        Assert.Equal(newRole, user.RoleOfUser);
        Assert.True(user.LastModifiedDate > oldModifiedDate);
    }
}