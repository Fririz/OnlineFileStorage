using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MassTransit;
using FileStorageService.Infrastructure.Consumers;

namespace FileStorageService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x => 
            {
                x.AddConsumer<FileDeletionRequestConsumer>();
                x.AddConsumer<FilesDeletionRequestConsumer>();
                x.UsingRabbitMq((context, cfg) => 
                {
                    cfg.Host("rabbit-mq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ReceiveEndpoint("file-deletion-request", e =>
                    {
                        e.ConfigureConsumer<FileDeletionRequestConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("files-deletion-request", e =>
                    {
                        e.ConfigureConsumer<FilesDeletionRequestConsumer>(context);
                    });
                });
            });
            var minioSection = configuration.GetSection("Minio");

            var endpoint  = (minioSection["Endpoint"]  ?? "minio:9000").Trim();
            var accessKey = (minioSection["AccessKey"] ?? "minio").Trim();
            var secretKey = (minioSection["SecretKey"] ?? "minio123").Trim();
            var useSslStr = (minioSection["UseSsl"]    ?? "false").Trim();

            var useSsl = false;
            bool.TryParse(useSslStr, out useSsl);

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("Minio:Endpoint is not configured.");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new InvalidOperationException("Minio:AccessKey is not configured.");
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("Minio:SecretKey is not configured.");

            services.AddSingleton<IMinioClient>(_ =>
            {
                var client = new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey);

                if (useSsl)
                {
                    client.WithSSL();
                }

                return client.Build();
            });

            services.AddScoped<IFileRepository, FileRepository>();

            return services;
        }
    }
}