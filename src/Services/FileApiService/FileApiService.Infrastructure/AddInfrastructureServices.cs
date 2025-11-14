using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FileApiService.Infrastructure.Persistence;
using FileApiService.Application.Contracts;
using FileApiService.Infrastructure.Repository;
using MassTransit;
namespace FileApiService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionString")));
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IAccessRightsRepository, AccessRightsRepository>();
        services.AddMassTransit(x => 
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