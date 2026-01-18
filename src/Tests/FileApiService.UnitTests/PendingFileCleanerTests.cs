using FileApiService.Application.BackgroundServices;
using FileApiService.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FileApiService.UnitTests;

public class PendingFileCleanerTests
{
    private readonly Mock<IItemRepository> _itemRepositoryMock;
    private readonly Mock<ILogger<PendingFileCleaner>> _loggerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;

    private readonly PendingFileCleaner _cleaner;

    public PendingFileCleanerTests()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _loggerMock = new Mock<ILogger<PendingFileCleaner>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();

        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_serviceScopeFactoryMock.Object);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IItemRepository)))
            .Returns(_itemRepositoryMock.Object);

        _cleaner = new PendingFileCleaner(_loggerMock.Object, _serviceProviderMock.Object);
    }

    [Fact]
    public async Task CleanFilesAsync_ShouldCallDeleteFiles_AndLogResult()
    {
        var deletedCount = 5;
        
        _itemRepositoryMock
            .Setup(repo => repo.DeleteFilesWithPendingExpired())
            .ReturnsAsync(deletedCount);

        await _cleaner.CleanFilesAsync(CancellationToken.None);

        _itemRepositoryMock.Verify(repo => repo.DeleteFilesWithPendingExpired(), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Deleted {deletedCount} items")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task CleanFilesAsync_IfErrorOccurs_ShouldLogError()
    {
        var exception = new Exception("Database failure");
        
        _itemRepositoryMock
            .Setup(repo => repo.DeleteFilesWithPendingExpired())
            .ThrowsAsync(exception);

        await _cleaner.CleanFilesAsync(CancellationToken.None);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error cleaning files")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}