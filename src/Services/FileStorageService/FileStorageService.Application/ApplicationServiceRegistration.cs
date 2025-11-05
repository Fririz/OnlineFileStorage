
using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
namespace FileStorageService.Application;

public static class ApplicationServiceRegistration
{
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<Consumers.FileDeletionRequestConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbit-mq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ReceiveEndpoint("file-deletion-request", e =>
                {
                    e.ConfigureConsumer<Consumers.FileDeletionRequestConsumer>(context);
                });

            });
        });

        services.AddScoped<IFileManager, FileManager>();
        services.AddScoped<ILinkGenerator, LinkGenerator>();
        services.AddScoped<ITokenManager, TokenManager>();
        return services;
    }
}