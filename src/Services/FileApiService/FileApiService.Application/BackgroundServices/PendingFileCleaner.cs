using FileApiService.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileApiService.Application.BackgroundServices; 
public class PendingFileCleaner : BackgroundService
{
    private readonly ILogger<PendingFileCleaner> _logger;
    private readonly IServiceProvider _serviceProvider; 

    public PendingFileCleaner(ILogger<PendingFileCleaner> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting cleaner");
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await CleanFilesAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    public async Task CleanFilesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope()) 
            {
                var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                    
                int deletedCount = await itemRepository.DeleteFilesWithPendingExpired();
                _logger.LogInformation($"Deleted {deletedCount} items.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning files.");
        }
    }
}