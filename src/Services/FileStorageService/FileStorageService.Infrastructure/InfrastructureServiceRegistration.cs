using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace FileStorageService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
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