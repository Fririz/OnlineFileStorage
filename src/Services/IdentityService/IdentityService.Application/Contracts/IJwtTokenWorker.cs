using System.Security.Claims;
using IdentityService.Domain.Entities;
namespace IdentityService.Application.Contracts;

public interface IJwtTokenWorker
{
    public string GenerateToken(User user);
    public ClaimsPrincipal? GetPrincipalFromToken(string? token);
}