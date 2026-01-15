
using FluentResults;
using IdentityService.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Contracts;

public interface IUserWorker
{
    public Task<Result> RegisterUser(UserAuthDto userDto, 
        CancellationToken cancellationToken = default);
    public Task<Result<string>> LoginUser(UserAuthDto userDto, 
        CancellationToken cancellationToken = default);

    public Result<UserTokenCheckDto> GetCurrentUser(string token,
        CancellationToken cancellationToken = default);
}