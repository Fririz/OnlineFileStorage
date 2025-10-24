using IdentityService.Domain.Entities;
namespace IdentityService.Application.Contracts;

public interface IJwtTokenWorker
{
    public string GenerateToken(User user);
    public bool CheckToken(string token);
}