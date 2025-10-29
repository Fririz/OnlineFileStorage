using FileApiService.Domain.Enums;
namespace FileApiService.Application.Dto;

public class ItemDeleteDto
{
    public Guid Id { get; set; }
    public TypeOfItem Type { get; set; }
}