// FileApiService.Domain.Entities/Item.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileApiService.Domain.Common;
using FileApiService.Domain.Enums;

namespace FileApiService.Domain.Entities;

[Table("Item")] 
public class Item : EntityBase
{
    //Item Id always equals object id in storage
    [Required]
    public Guid OwnerId { get; private set; }

    public Guid? ParentId { get; private set; } 

    [Required]
    public TypeOfItem Type { get; private set; } 

    [Required]
    [MaxLength(255)] 
    public string Name { get; private set; }

    public long? FileSize { get; private set; } 

    [MaxLength(255)]
    public string? MimeType { get; private set; } 
    
    public UploadStatus? Status { get; private set; } 

    // Logic Fields

    public bool IsDeleted { get; private set; } = false; 

    
    [ForeignKey(nameof(ParentId))]
    public virtual Item? Parent { get; private set; } 
    
    public virtual ICollection<Item> Children { get; private set; } = new HashSet<Item>(); 

    private Item() : base() { }
    
    public static Item CreateFolder(Guid ownerId, string name, Guid? parentId)
    {
        return new Item
        {
            OwnerId = ownerId,
            ParentId = parentId,
            Type = TypeOfItem.Folder,
            Name = name,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
        };
    }
    
    public static Item CreateFile(Guid ownerId, string name, Guid? parentId)
    {
        return new Item
        {
            OwnerId = ownerId,
            ParentId = parentId,
            Type = TypeOfItem.File,
            Name = name,
            Status = UploadStatus.Pending, 
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
        };
    }

    
    public void CompleteUpload(long fileSize, string mimeType)
    {
        if (Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Cannot complete upload for a folder.");
        }
        
        FileSize = fileSize;
        MimeType = mimeType;
        Status = UploadStatus.Ready; 
        LastModifiedDate = DateTime.UtcNow;
    }
    
    public void MarkUploadAsFailed()
    {
        if (Type != TypeOfItem.File)
        {
            throw new InvalidOperationException("Cannot mark upload as failed for a folder.");
        }
        Status = UploadStatus.Failed;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        }
        Name = newName;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Move(Guid? newParentId)
    {
        if (newParentId == ParentId)
        {
            throw new InvalidOperationException("Cannot move to the same folder.");
        }
        ParentId = newParentId;
        LastModifiedDate = DateTime.UtcNow;
    }
    
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        LastModifiedDate = DateTime.UtcNow;
    }
}