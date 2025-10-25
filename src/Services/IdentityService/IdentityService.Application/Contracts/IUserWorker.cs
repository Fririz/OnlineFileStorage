
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Contracts;

public interface IUserWorker
{
    public Task<Guid> RegisterUser(UserDto userDto);
    public Task<string?> LoginUser(UserDto userDto);
}