namespace IdentityService.Domain.Common;

public class EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}