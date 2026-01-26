using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IItemService
{
    public Task<Result<List<ItemResponseDto>>> FindItem(string searchQuery, Guid userId,
        CancellationToken cancellationToken = default);
    public Task<Result<List<ItemResponseDto>>> GetRootItems(Guid userId);
    public Task<Item?> GetParent(Guid itemId);
}