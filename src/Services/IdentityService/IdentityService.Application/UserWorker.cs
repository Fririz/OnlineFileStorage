using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using IdentityService.Application.Exceptions;
using IdentityService.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using FluentResults;
using IdentityService.Application.Exceptions.FluentResultsErrors;

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
    public async Task<Result> RegisterUser(UserAuthDto userDto, CancellationToken cancellationToken = default)
    {
        if(await _userRepository.CheckUserExistenceAsync(userDto.Username, cancellationToken))
        {
            _logger.LogInformation($"User {userDto.Username} already exists.");
            return Result.Fail(new UserAlreadyExistsError($"User {userDto.Username} already exists."));
        }
        
        var user = User.CreateUser(userDto.Username, _passwordWorker.HashPassword(userDto.Password), Role.User);
        await _userRepository.AddUserAsync(user, cancellationToken);
        _logger.LogInformation($"User {user.Username} registered successfully.");
        return Result.Ok();

    }

    public async Task<Result<string>> LoginUser(UserAuthDto userDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByUsernameAsync(userDto.Username, cancellationToken);
        if (user == null || !_passwordWorker.CheckPassword(userDto.Password, user.PasswordHash))
        {
            _logger.LogInformation($"User {userDto.Username} failed to login.");
            return Result.Fail("Invalid username or password");
        }
        _logger.LogInformation($"User {user.Username} logged in successfully.");
        var token = _jwtTokenWorker.GenerateToken(user);
        return Result.Ok(token);
    }
}