using FileApiService.Application.Contracts;
using FileApiService.Domain.Entities;
using FileApiService.Application.Dto;
namespace FileApiService.Application;

public class Mapper : IMapper
{
    public ItemResponseDto Map(Item item)
    {
        return new ItemResponseDto
        {
            Id = item.Id,
            OwnerId = item.OwnerId,
            ParentId = item.ParentId,
            Type = item.Type,
            Name = item.Name,
            FileSize = item.FileSize,
            Status = item.Status
        };
    }
    public List<ItemResponseDto> Map(List<Item> items)
    {
        return items.Select(item => Map(item)).ToList();
    } 
    public List<ItemResponseDto> Map(IEnumerable<Item?> items)
    {
        if (items == null)
        {
            return new List<ItemResponseDto>();
        }

        return items
            .Where(item => item != null) 
            .Select(item => Map(item!))   
            .ToList();
    }
}
