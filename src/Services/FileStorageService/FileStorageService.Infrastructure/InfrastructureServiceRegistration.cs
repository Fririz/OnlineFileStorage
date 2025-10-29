using FileStorageService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;


namespace FileStorageService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(options =>
            configuration.GetSection(MinioOptions.Section).Bind(options));
        services.AddSingleton<IMinioClient>(
            sp =>
            {
                var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    
                return new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(options.AccessKey, options.SecretKey)
                    .WithSSL(options.UseSsl)
                    .Build();
            });
        services.AddScoped<IFileRepository, FileRepository>();
        return services;
    }
}