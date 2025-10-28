using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FileApiService.Infrastructure.Persistence;
using FileApiService.Application.Contracts;
using FileApiService.Infrastructure.Repository;

namespace FileApiService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionString")));
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IAccessRightsRepository, AccessRightsRepository>();

        return services;
    }
}