using IdentityService.Domain.Entities;
namespace IdentityService.Application.Contracts;

public interface IJwtTokenGenerator
{
    public string GenerateToken(User user);
}