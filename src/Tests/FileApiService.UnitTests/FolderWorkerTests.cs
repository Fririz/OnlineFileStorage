using Contracts.Shared;
using FileApiService.Application;
using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FileApiService.UnitTests;

public class FolderWorkerTests
{
    private readonly Mock<IItemRepository> _itemRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<FolderWorker>> _loggerMock;

    private readonly FolderWorker _folderWorker;

    public FolderWorkerTests(ITestOutputHelper testOutputHelper)
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<FolderWorker>>();

        _folderWorker = new FolderWorker(
            _loggerMock.Object,
            _itemRepositoryMock.Object,
            _publishEndpointMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetChildrenAsync_Success_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
        var folder = Item.CreateFolder(userId, "testFolder", null);
        var file = Item.CreateFile(userId, "testFile", folder.Id);
        
        var itemsList = new List<Item> { file };
        
        var dtos = new List<ItemResponseDto> 
        { 
            new ItemResponseDto 
            { 
                Name = file.Name, 
                Id = file.Id, 
                FileSize = file.FileSize, 
                OwnerId = file.OwnerId, 
                ParentId = file.ParentId, 
                Status = file.Status, 
                Type = file.Type
            } 
        };

        _itemRepositoryMock.Setup(repo => repo.GetAllChildrenAsync(userId))
            .ReturnsAsync(itemsList);
    
        _mapperMock.Setup(m => m.Map(It.IsAny<IEnumerable<Item>>()))
            .Returns(dtos);

        var result = await _folderWorker.GetChildrenAsync(userId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value); 
        Assert.Equal(dtos, result.Value);
    }

    [Fact]
    public async Task CreateFolder_Success_ShouldReturnId()
    {
        var userId = Guid.NewGuid();
        var dto = new ItemCreateDto { Name = "NewFolder", Type = TypeOfItem.Folder };

        var result = await _folderWorker.CreateFolder(dto, userId);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        
        _itemRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Item>(i => i.Type == TypeOfItem.Folder && i.Name == "NewFolder"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFolder_IfTypeIsFile_ReturnsInvalidOperationError()
    {
        var userId = Guid.NewGuid();
        var dto = new ItemCreateDto { Name = "File.txt", Type = TypeOfItem.File };

        var result = await _folderWorker.CreateFolder(dto, userId);

        Assert.True(result.IsFailed);
        Assert.IsType<InvalidOperationError>(result.Errors.First());
        
        _itemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task DeleteFolder_IfNotFound_ReturnsDirectoryNotFoundError()
    {
        var folderId = Guid.NewGuid();
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(folderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        var result = await _folderWorker.DeleteFolderWithAllChildren(folderId, Guid.NewGuid());

        Assert.True(result.IsFailed);
        Assert.IsType<DirectoryNotFoundError>(result.Errors.First());
    }

    [Fact]
    public async Task DeleteFolder_IfItemIsFile_ReturnsInvalidOperationError()
    {
        var folderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var fileItem = Item.CreateFile(userId, "wrong.txt", null); 
        
        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(folderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileItem);

        var result = await _folderWorker.DeleteFolderWithAllChildren(folderId, userId);

        Assert.True(result.IsFailed);
        Assert.IsType<InvalidOperationError>(result.Errors.First());
    }

    [Fact]
    public async Task DeleteFolder_IfWrongOwner_ReturnsUnauthorizedError()
    {
        var folderId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var alienId = Guid.NewGuid();
        
        var folder = Item.CreateFolder(ownerId, "MyFolder", null);

        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(folderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(folder);

        var result = await _folderWorker.DeleteFolderWithAllChildren(folderId, alienId);

        Assert.True(result.IsFailed);
        Assert.IsType<UnauthorizedAccessError>(result.Errors.First());
    }
    
    [Fact]
    public async Task DeleteFolder_Recursive_ShouldDeleteAllDescendantsAndPublishEvent()
    {
        var userId = Guid.NewGuid();

        var rootFolder = Item.CreateFolder(userId, "Root", null);
        var rootId = rootFolder.Id;

        var childFile = Item.CreateFile(userId, "ChildFile.txt", rootId);
        var childFolder = Item.CreateFolder(userId, "ChildFolder", rootId);
        
        var grandChildFile = Item.CreateFile(userId, "GrandChild.txt", childFolder.Id);

        _itemRepositoryMock.Setup(repo => repo.GetByIdAsync(rootId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rootFolder);

        _itemRepositoryMock.Setup(repo => repo.GetAllChildrenAsync(rootId))
            .ReturnsAsync(new List<Item> { childFile, childFolder });

        _itemRepositoryMock.Setup(repo => repo.GetAllChildrenAsync(childFolder.Id))
            .ReturnsAsync(new List<Item> { grandChildFile });

        var result = await _folderWorker.DeleteFolderWithAllChildren(rootId, userId);

        Assert.True(result.IsSuccess);

        _itemRepositoryMock.Verify(repo => repo.UpdateRangeAsync(
            It.Is<IEnumerable<Item>>(list => 
                list.Count() == 4 && 
                list.All(i => i.IsDeleted)
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _publishEndpointMock.Verify<Task>(p => p.Publish(
            It.Is<FilesDeletionRequest>(msg => 
                msg.IdsToDelete.Count() == 4 &&
                msg.IdsToDelete.Contains(rootId) &&
                msg.IdsToDelete.Contains(grandChildFile.Id)
            ), 
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}