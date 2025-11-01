using FileApiService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FileApiService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileWorker, FileWorker>();
        services.AddScoped<IFolderWorker, FolderWorker>();
        services.AddScoped<ISerializer, Serializer>();
        return services;
    }
}