using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;

namespace FileApiService.Application.Contracts;

public interface IMapper
{
    public ItemResponseDto Map(Item item);
    public List<ItemResponseDto> Map(IEnumerable<Item?>? items);
}