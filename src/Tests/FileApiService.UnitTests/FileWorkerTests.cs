using FileApiService.Application;
using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using Contracts.Shared;
using FluentResults;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FileApiService.UnitTests;

public class FileWorkerTests
{
    private readonly Mock<IItemRepository> _itemRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILinkProvider> _linkProviderMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<FileWorker>> _loggerMock;

    private readonly FileWorker _fileWorker;

    public FileWorkerTests()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _linkProviderMock = new Mock<ILinkProvider>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<FileWorker>>();
        
        _fileWorker = new FileWorker(
            _loggerMock.Object,
            _itemRepositoryMock.Object,
            _publishEndpointMock.Object,
            _mapperMock.Object,
            _linkProviderMock.Object
        );
    }
    [Fact]
    public async Task DownloadFile_Success_ShouldReturnLink()
    {
        var userId = Guid.NewGuid();
        Item file = Item.CreateFile(userId, "test.txt", null);
        var fileId = file.Id;
        string expectedLink = "https://test/myfile";

        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _linkProviderMock.Setup(lp => lp.GetDownloadLinkAsync(fileId, file.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLink);

        var result = await _fileWorker.DownloadFile(fileId, userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedLink, result.Value); 
    }
    [Fact]
    public async Task DownloadFile_ItemNotFound_ReturnsFileNotFoundError()
    {
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        var result = await _fileWorker.DownloadFile(fileId, userId);

        Assert.True(result.IsFailed); 
        Assert.IsType<FileNotFoundError>(result.Errors.First()); 
    }

    [Fact]
    public async Task DownloadFile_WrongOwner_ReturnsUnauthorizedError()
    {
        var fileId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var alienUserId = Guid.NewGuid(); 

        var fileItem = Item.CreateFile(ownerId, "test.txt", null);
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileItem);

        var result = await _fileWorker.DownloadFile(fileId, alienUserId);

        Assert.True(result.IsFailed);
        Assert.IsType<UnauthorizedAccessError>(result.Errors.First());
    }
    
    //Create file tests
    [Fact]
    public async Task CreateFile_Success_ReturnsUploadLink()
    {
        var userId = Guid.NewGuid();
        var dto = new ItemCreateDto { Name = "vacation.png", Type = TypeOfItem.File };
        string expectedLink = "https://s3.bucket/upload-url";

        _linkProviderMock.Setup(lp => lp.GetUploadLinkAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLink);

        var result = await _fileWorker.CreateFile(dto, userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedLink, result.Value);

        _itemRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Item>(i => i.Name == dto.Name && i.OwnerId == userId),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    
        _itemRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task CreateFile_IfTypeIsFolder_ReturnsInvalidOperationError()
    {
        var userId = Guid.NewGuid();
        var dto = new ItemCreateDto { Name = "NewFolder", Type = TypeOfItem.Folder }; 

        var result = await _fileWorker.CreateFile(dto, userId);

        Assert.True(result.IsFailed);
        Assert.IsType<InvalidOperationError>(result.Errors.First());
    
        _itemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task CreateFile_IfLinkProviderFails_ShouldDeleteFileFromRepo()
    {

        var userId = Guid.NewGuid();
        var dto = new ItemCreateDto { Name = "test.jpg", Type = TypeOfItem.File };

        _linkProviderMock.Setup(lp => lp.GetUploadLinkAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("S3 Service Unavailable"));

        await Assert.ThrowsAsync<Exception>(() => _fileWorker.CreateFile(dto, userId));

        _itemRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    //Delete file tests
    [Fact]
    public async Task DeleteFile_Success_ShouldPublishEvent()
    {
        var userId = Guid.NewGuid();
    
        var fileItem = Item.CreateFile(userId, "todelete.txt", null);
    
        var fileId = fileItem.Id; 

        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileItem);

        var result = await _fileWorker.DeleteFile(fileId, userId);

        Assert.True(result.IsSuccess);
        _publishEndpointMock.Verify(p => p.Publish(
            It.Is<FileDeletionRequested>(msg => msg.FileId == fileId), 
            It.IsAny<CancellationToken>()
        ), Times.Once);
    
        _itemRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFile_IfFileNotFound_ReturnFailedResult()
    {
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        var result = await _fileWorker.DeleteFile(fileId, userId);
        
        Assert.True(result.IsFailed);
        Assert.IsType<FileNotFoundError>(result.Errors.First());
    }
    [Fact]
    public async Task DeleteFile_IfUnauthorizedAccess_ReturnFailedResult()
    {
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Item file = Item.CreateFile(Guid.NewGuid(), "testFile", null);
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);

        var result = await _fileWorker.DeleteFile(fileId, userId);
        
        Assert.True(result.IsFailed);
        Assert.IsType<UnauthorizedAccessError>(result.Errors.First());
    }
    [Fact]
    public async Task DeleteFile_IfInvalidOperation_ReturnFailedResult()
    {
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        Item folder = Item.CreateFolder(userId, "testFolder", null);
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(folder);

        var result = await _fileWorker.DeleteFile(fileId, userId);
        
        Assert.True(result.IsFailed);
        Assert.IsType<InvalidOperationError>(result.Errors.First());
    }
    [Fact]
    public async Task GetRootItems_Success_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
    
        var items = new List<Item> { Item.CreateFile(userId, "file1.txt", null) };
    
        var dtos = new List<ItemResponseDto> { new ItemResponseDto { Name = "file1.txt" } };

        _itemRepositoryMock.Setup(repo => repo.GetRootItems(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _mapperMock.Setup(m => m.Map(items)).Returns(dtos); 

        var result = await _fileWorker.GetRootItems(userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtos, result.Value);
    
        _mapperMock.Verify(m => m.Map(items), Times.Once);
    }
}