// FileApiService.Domain.Common/EntityBase.cs
using System.ComponentModel.DataAnnotations;

namespace FileApiService.Domain.Common;

public abstract class EntityBase
{
    [Required]
    public Guid Id { get; private set; } 

    [Required] 
    public DateTime CreatedDate { get; protected set; } 

    public DateTime? LastModifiedDate { get; set; } 

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }
}