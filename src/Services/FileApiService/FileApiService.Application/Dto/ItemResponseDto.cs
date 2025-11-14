using FileApiService.Domain.Enums;
namespace FileApiService.Application.Dto;

public class ItemResponseDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public Guid? ParentId { get;  set; } 

    public TypeOfItem Type { get; set; } 
    
    public string Name { get; set; }

    public long? FileSize { get;  set; } 
    
    public UploadStatus? Status { get; set; } 

}