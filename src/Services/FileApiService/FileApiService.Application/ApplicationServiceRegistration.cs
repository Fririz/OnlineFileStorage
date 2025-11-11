using FileApiService.Application.BackgroundServices;
using FileApiService.Application.Contracts;
using MassTransit;
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
        services.AddHttpClient();
        services.AddHostedService<PendingFileCleaner>();
        services.AddMassTransit(x =>  //TODO: move rabbitmq to another layer
        {
            x.AddConsumer<Consumers.FileUploadCompletedConsumer>();
            x.AddConsumer<Consumers.FileUploadFailedConsumer>();
            x.AddConsumer<Consumers.FileDeletionCompleteConsumer>();
            x.AddConsumer <Consumers.FilesDeletionCompleteConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbit-mq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ReceiveEndpoint("notifications-file-uploaded", e =>
                {
                    e.ConfigureConsumer<Consumers.FileUploadCompletedConsumer>(context);
                });
                
                cfg.ReceiveEndpoint("notifications-file-failed", e =>
                {
                    e.ConfigureConsumer<Consumers.FileUploadFailedConsumer>(context);
                });
                cfg.ReceiveEndpoint("notifications-file-deletion-complete", e =>
                {
                    e.ConfigureConsumer<Consumers.FileDeletionCompleteConsumer>(context);
                });
                cfg.ReceiveEndpoint("notifications-files-deletion-complete", e =>
                {
                    e.ConfigureConsumer<Consumers.FilesDeletionCompleteConsumer>(context);
                });
            });
        });
        return services;
    }
}