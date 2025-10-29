
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Contracts;

public interface IUserWorker
{
    public Task<Guid> RegisterUser(UserAuthDto userDto);
    public Task<string?> LoginUser(UserAuthDto userDto);
}