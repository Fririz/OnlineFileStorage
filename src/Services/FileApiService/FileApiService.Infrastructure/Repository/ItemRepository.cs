using FileApiService.Application.Contracts;
using FileApiService.Domain.Entities;
using FileApiService.Domain.Enums;
using FileApiService.Infrastructure.Persistence;
using IdentityService.Application.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileApiService.Infrastructure.Repository;

public class ItemRepository : RepositoryBase<Item>, IItemRepository
{
    public ItemRepository(Context context) : base(context)
    {
        
    }
    public IEnumerable<Item?> GetAllChildren(Guid itemId)
    {
        return _context.Items.Where(i => i.ParentId == itemId);
    }

    public Item? GetParent(Guid itemId)
    {
        return _context.Items.Where(i => itemId == i.Id).
            Select(i => i.Parent).FirstOrDefault();
    }

    public IEnumerable<Item?> GetRootItems(Guid userId)
    {
        return _context.Items.Where(i => i.ParentId == null).
            Where(i => i.OwnerId == userId);
    }

    public IEnumerable<Item?> GetSharedRootItems(Guid userId)
    {
        var accessibleItemIds = _context.AccessRights
            .Where(ar => ar.UserId == userId && ar.CanRead) 
            .Select(ar => ar.ItemId);
        return _context.Items
            .Where(i => accessibleItemIds.Contains(i.Id))
            .Where(i => i.ParentId == null)
            .Where(i => i.OwnerId != userId);
    }

    public async Task<int> DeleteFilesWithPendingExpired()
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        var rowsAffected = await _context.Items
            .Where(f => f.Type == TypeOfItem.File && f.Status == UploadStatus.Pending && f.CreatedDate < oneHourAgo)
            .ExecuteDeleteAsync();

        return rowsAffected;
    }
}