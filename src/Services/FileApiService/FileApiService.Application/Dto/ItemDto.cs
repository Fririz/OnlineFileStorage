using FileApiService.Domain.Enums;
namespace FileApiService.Application.Dto;

public class ItemDto
{
    public string Name { get; set; }
    public Guid ParentId { get; set; }
    public TypeOfItem Type { get; set; }
}