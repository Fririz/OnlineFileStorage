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
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration files.");
        }
        services.AddDbContext<UserContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IUserRepository, UserRepository>();


        return services;
    }
}