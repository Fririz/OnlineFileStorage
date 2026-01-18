using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using Xunit;

namespace FileApiService.UnitTests;

public class ItemTests
{
    [Fact]
    public void CreateFolder_ShouldInitializeCorrectly()
    {
        var ownerId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var name = "MyFolder";

        var folder = Item.CreateFolder(ownerId, name, parentId);

        Assert.NotEqual(Guid.Empty, folder.Id); 
        Assert.Equal(ownerId, folder.OwnerId);
        Assert.Equal(parentId, folder.ParentId);
        Assert.Equal(name, folder.Name);
        Assert.Equal(TypeOfItem.Folder, folder.Type);
        Assert.False(folder.IsDeleted);
        Assert.Null(folder.Status);
        Assert.NotEqual(default, folder.CreatedDate);
    }

    [Fact]
    public void CreateFile_ShouldInitializeWithPendingStatus()
    {
        var ownerId = Guid.NewGuid();
        var name = "document.pdf";

        var file = Item.CreateFile(ownerId, name, null);

        Assert.Equal(TypeOfItem.File, file.Type);
        Assert.Equal(UploadStatus.Pending, file.Status);
        Assert.Equal(name, file.Name);
        Assert.False(file.IsDeleted);
    }


    [Fact]
    public void CompleteUpload_IfItemIsFile_ShouldUpdateStatusAndSize()
    {
        var file = Item.CreateFile(Guid.NewGuid(), "test.jpg", null);
        long size = 1024;
        string mime = "image/jpeg";
        var dateBefore = file.LastModifiedDate;

        file.CompleteUpload(size, mime);

        Assert.Equal(UploadStatus.Ready, file.Status);
        Assert.Equal(size, file.FileSize);
        Assert.Equal(mime, file.MimeType);
        Assert.NotNull(file.LastModifiedDate);
        
        Assert.True(file.LastModifiedDate > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void CompleteUpload_IfItemIsFolder_ShouldThrowException()
    {
        var folder = Item.CreateFolder(Guid.NewGuid(), "Folder", null);

        var exception = Assert.Throws<InvalidOperationException>(() => 
            folder.CompleteUpload(500, "text/plain"));
        
        Assert.Equal("Cannot complete upload for a folder.", exception.Message);
    }


    [Fact]
    public void MarkUploadAsFailed_IfItemIsFile_ShouldSetStatusFailed()
    {
        var file = Item.CreateFile(Guid.NewGuid(), "bad_file.exe", null);

        file.MarkUploadAsFailed();

        Assert.Equal(UploadStatus.Failed, file.Status);
        Assert.NotNull(file.LastModifiedDate);
    }

    [Fact]
    public void MarkUploadAsFailed_IfItemIsFolder_ShouldThrowException()
    {
        var folder = Item.CreateFolder(Guid.NewGuid(), "Folder", null);

        Assert.Throws<InvalidOperationException>(() => folder.MarkUploadAsFailed());
    }


    [Fact]
    public void Rename_ValidName_ShouldUpdateName()
    {
        var item = Item.CreateFile(Guid.NewGuid(), "old_name.txt", null);
        string newName = "new_name.txt";

        item.Rename(newName);

        Assert.Equal(newName, item.Name);
        Assert.NotNull(item.LastModifiedDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Rename_InvalidName_ShouldThrowArgumentException(string? badName)
    {
        var item = Item.CreateFile(Guid.NewGuid(), "file.txt", null);

        Assert.Throws<ArgumentException>(() => item.Rename(badName));
    }



    [Fact]
    public void Move_ToNewParent_ShouldUpdateParentId()
    {
        var oldParent = Guid.NewGuid();
        var newParent = Guid.NewGuid();
        var item = Item.CreateFile(Guid.NewGuid(), "file.txt", oldParent);

        item.Move(newParent);

        Assert.Equal(newParent, item.ParentId);
        Assert.NotNull(item.LastModifiedDate);
    }

    [Fact]
    public void Move_ToSameParent_ShouldThrowException()
    {
        var parentId = Guid.NewGuid();
        var item = Item.CreateFile(Guid.NewGuid(), "file.txt", parentId);

        Assert.Throws<InvalidOperationException>(() => item.Move(parentId));
    }


    [Fact]
    public void MarkAsDeleted_ShouldSetIsDeletedTrue()
    {
        var item = Item.CreateFile(Guid.NewGuid(), "todelete.txt", null);

        item.MarkAsDeleted();

        Assert.True(item.IsDeleted);
        Assert.NotNull(item.LastModifiedDate);
    }
}