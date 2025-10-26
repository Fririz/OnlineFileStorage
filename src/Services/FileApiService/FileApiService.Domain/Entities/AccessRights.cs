using System.ComponentModel.DataAnnotations;
using FileApiService.Domain.Common;
using FileApiService.Domain.Enums;
namespace FileApiService.Domain.Entities;

public class AccessRights : EntityBase
{
    [Required]
    public Guid UserId {get; private set;}
    [Required]
    public Guid ItemId { get; private set; }
    [Required]
    public bool CanRead { get; private set; }
    public bool CanDownload { get; private set; }
    public bool CanEdit { get; private set; }
    
    private AccessRights() : base()
    {
        
    }

    public static AccessRights Create(Guid userId, Guid itemId, 
        bool canRead, bool canDownload, bool canEdit)
    {
        return new AccessRights
        {
            UserId = userId,
            ItemId = itemId,
            CanRead = canRead,
            CanDownload = canDownload,
            CanEdit = canEdit,
            LastModifiedDate = DateTime.UtcNow,
        };
    }
    public void UpdateRights(bool canRead, bool canDownload, bool canEdit)
    {
        CanRead = canRead;
        CanDownload = canDownload;
        CanEdit = canEdit;
        LastModifiedDate = DateTime.UtcNow;
    }
    
}