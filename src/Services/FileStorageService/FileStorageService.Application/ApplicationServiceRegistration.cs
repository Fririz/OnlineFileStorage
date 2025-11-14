
using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
namespace FileStorageService.Application;

public static class ApplicationServiceRegistration
{
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileManager, FileManager>();
        services.AddScoped<ILinkGenerator, LinkGenerator>();
        services.AddScoped<ITokenManager, TokenManager>();
        return services;
    }
}