using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FileStorageService.Application.Contracts;

namespace FileStorageService.Infrastructure;

public class MinioBucketInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MinioBucketInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var fileRepository = scope.ServiceProvider.GetRequiredService<IFileRepository>();

        if (fileRepository is FileRepository concreteRepo)
        {
            await concreteRepo.EnsureBucketExistsAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}