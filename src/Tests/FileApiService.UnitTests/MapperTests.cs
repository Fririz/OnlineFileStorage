using FileApiService.Application;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using Xunit;

namespace FileApiService.UnitTests;

public class MapperTests
{
    private readonly Mapper _mapper;

    public MapperTests()
    {
        _mapper = new Mapper();
    }

    [Fact]
    public void Map_SingleItem_ShouldMapAllFieldsCorrectly()
    {
        var userId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var item = Item.CreateFile(userId, "test.jpg", parentId);
        

        var result = _mapper.Map(item);

        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal(item.Name, result.Name);
        Assert.Equal(item.OwnerId, result.OwnerId);
        Assert.Equal(item.ParentId, result.ParentId);
        Assert.Equal(item.Type, result.Type);
        Assert.Equal(item.Status, result.Status);
        Assert.Equal(item.FileSize, result.FileSize);
    }

    [Fact]
    public void Map_List_ShouldMapAllItems()
    {
        var userId = Guid.NewGuid();
        var file = Item.CreateFile(userId, "file.txt", null);
        var folder = Item.CreateFolder(userId, "folder", null);
        
        var items = new List<Item> { file, folder };

        var result = _mapper.Map(items);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        
        Assert.Equal(file.Id, result[0].Id);
        Assert.Equal(file.Name, result[0].Name);
        
        Assert.Equal(folder.Id, result[1].Id);
        Assert.Equal(folder.Name, result[1].Name);
        Assert.Equal(TypeOfItem.Folder, result[1].Type);
    }

    [Fact]
    public void Map_IEnumerable_WithNulls_ShouldFilterNullsAndMapRest()
    {
        var userId = Guid.NewGuid();
        var file = Item.CreateFile(userId, "valid.txt", null);
        
        var items = new List<Item?> { file, null };

        var result = _mapper.Map(items);
        
        Assert.NotNull(result);
        Assert.Single(result); 
        Assert.Equal(file.Id, result.First().Id);
    }

    [Fact]
    public void Map_IEnumerable_NullInput_ShouldReturnEmptyList()
    {
        IEnumerable<Item?>? nullCollection = null;

        var result = _mapper.Map(nullCollection);

        Assert.NotNull(result);
        Assert.Empty(result); 
    }

    [Fact]
    public void Map_IEnumerable_EmptyInput_ShouldReturnEmptyList()
    {
        var emptyList = new List<Item?>();

        var result = _mapper.Map(emptyList);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}