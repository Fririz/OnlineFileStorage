using FileApiService.Application.BackgroundServices;
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
        services.AddScoped<IMapper, Mapper>();
        services.AddHttpClient(); //TODO move to infr layer and change to grpc
        services.AddHostedService<PendingFileCleaner>();
        return services;
    }
}