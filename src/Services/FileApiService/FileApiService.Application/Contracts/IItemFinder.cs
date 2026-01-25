using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using FluentResults;

namespace FileApiService.Application.Contracts;

public interface IItemFinder
{
    public Task<Result<List<ItemResponseDto>>> FindItem(string searchQuery, Guid userId,
        CancellationToken cancellationToken = default);
}