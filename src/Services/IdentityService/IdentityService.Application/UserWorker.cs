using IdentityService.Application.Contracts;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IdentityService.Application;

public class UserWorker : IUserWorker
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordWorker _passwordWorker;
    public UserWorker(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordWorker passwordWorker)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordWorker = passwordWorker;
    }
    public async Task<IResult> RegisterUser(UserDto userDto)
    {
        if(await _userRepository.CheckUserExistenceAsync(userDto.Username))
        {
            return Results.BadRequest<string>("User with this username already exists.");
        }

        var user = new User(userDto.Username, _passwordWorker.HashPassword(userDto.Password));
        await _userRepository.AddUserAsync(user);
        return Results.Ok(user.Id);

    }

    public async Task<string?> Login(UserDto userDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(userDto.Username);
        if (user == null)
        {
            return null;
        }
        if (!_passwordWorker.CheckPassword(userDto.Password, user.PasswordHash))
        {
            return null;
        }

        return _jwtTokenGenerator.GenerateToken(user);
    }
}