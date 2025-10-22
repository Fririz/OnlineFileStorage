
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Contracts;

public interface IUserWorker
{
    public Task<IResult> RegisterUser(UserDto userDto);
    public Task<string?> Login(UserDto userDto);
}