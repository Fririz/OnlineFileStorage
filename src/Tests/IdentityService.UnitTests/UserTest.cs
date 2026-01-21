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
}