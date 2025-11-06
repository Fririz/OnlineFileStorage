using FileApiService.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 

namespace FileApiService.Application.BackgroundServices;

public class PendingFileCleaner : BackgroundService
{
    private readonly ILogger<PendingFileCleaner> _logger;
        // not standart di bcs itemrepository scoped and registered as scoped(this is singleton)
    private readonly IServiceProvider _serviceProvider; 

    public PendingFileCleaner(ILogger<PendingFileCleaner> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pending file cleaner is starting.");
        
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                        
                        int deletedCount = await itemRepository.DeleteFilesWithPendingExpired();
                        _logger.LogInformation("Pending file cleanup finished. Deleted {Count} items.", deletedCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up pending files.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pending file cleaner is stopping.");
        }
    }
}