using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FileApiService.Infrastructure.Persistence;
using FileApiService.Application.Contracts;
using FileApiService.Infrastructure.Repository;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using OnlineFileStorage.Grpc.Shared.Storage;

namespace FileApiService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IAccessRightsRepository, AccessRightsRepository>();
        services.AddScoped<ILinkProvider, Grpc.LinkProvider>();
        services.AddGrpcClient<StorageService.StorageServiceClient>(o =>
            {
                o.Address = new Uri("host.docker.internal:8083");
            }
        );
        var rabbitMqSection = configuration.GetSection("RabbitMq");                            
        var rabbitMqUser= (rabbitMqSection["User"] ?? throw new InvalidOperationException("RabbitMq:User is not set")).Trim();                         
        var rabbitMqPass = (rabbitMqSection["Pass"] ?? throw new InvalidOperationException("RabbitMq:Pass is not set")).Trim();                        
        var rabbitMqHost = (rabbitMqSection["Host"] ?? throw new InvalidOperationException("RabbitMq:Host is not set")).Trim();    
        
        services.AddMassTransit(x => 
        {
            x.AddEntityFrameworkOutbox<Context>(o =>
            {
                o.UseBusOutbox(); 
                o.LockStatementProvider = new PostgresLockStatementProvider(false);
            });
            x.AddConsumer<Consumers.FileUploadCompletedConsumer>();
            x.AddConsumer<Consumers.FileUploadFailedConsumer>();
            x.AddConsumer<Consumers.FileDeletionCompleteConsumer>();
            x.AddConsumer <Consumers.FilesDeletionCompleteConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
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
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}