using FileApiService.Application.BackgroundServices;
using FileApiService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FileApiService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFolderService, FolderService>();
        services.AddScoped<ISerializer, Serializer>();
        services.AddScoped<IMapper, Mapper>();
        services.AddScoped<IItemService, ItemService>();
        services.AddHostedService<PendingFileCleaner>();
        return services;
    }
}