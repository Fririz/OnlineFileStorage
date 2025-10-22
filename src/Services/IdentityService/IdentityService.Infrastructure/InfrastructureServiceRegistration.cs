using IdentityService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Repository;

namespace IdentityService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionString")));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}