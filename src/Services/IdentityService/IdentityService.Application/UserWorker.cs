using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IdentityService.Application;

public class UserWorker : IUserWorker
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenWorker _jwtTokenWorker;
    private readonly IPasswordWorker _passwordWorker;
    private readonly ILogger _logger;
    public UserWorker(IUserRepository userRepository, 
        IJwtTokenWorker jwtTokenWorker, 
        IPasswordWorker passwordWorker,
        ILogger<UserWorker> logger)
    {
        _userRepository = userRepository;
        _jwtTokenWorker = jwtTokenWorker;
        _passwordWorker = passwordWorker;
        _logger = logger;
    }
    public async Task<IResult> RegisterUser(UserDto userDto)
    {
        if(await _userRepository.CheckUserExistenceAsync(userDto.Username))
        {
            _logger.LogInformation($"User {userDto.Username} already exists.");
            return Results.BadRequest<string>("User with this username already exists.");
        }

        var user = new User(userDto.Username, _passwordWorker.HashPassword(userDto.Password));
        await _userRepository.AddUserAsync(user);
        _logger.LogInformation($"User {user.Username} registered successfully.");
        return Results.Ok(user.Id);

    }

    public async Task<string?> LoginUser(UserDto userDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(userDto.Username);
        if (user == null || !_passwordWorker.CheckPassword(userDto.Password, user.PasswordHash))
        {
            _logger.LogInformation($"User {userDto.Username} failed to login.");
            return null;
        }
        _logger.LogInformation($"User {user.Username} logged in successfully.");
        return _jwtTokenWorker.GenerateToken(user);
    }
}