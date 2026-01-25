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
    
    public async Task<IEnumerable<Item?>> GetAllChildrenAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.Items.Where(i => i.ParentId == itemId).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Item?> GetParent(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.Items.Where(i => itemId == i.Id).
            Select(i => i.Parent).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Item?>> GetRootItems(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Items.Where(i => i.ParentId == null).
            Where(i => i.OwnerId == userId).AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Item?>> GetSharedRootItems(Guid userId, CancellationToken cancellationToken = default)
    {
        var accessibleItemIds = _context.AccessRights
            .Where(ar => ar.UserId == userId && ar.CanRead) 
            .Select(ar => ar.ItemId);
        return await _context.Items
            .Where(i => accessibleItemIds.Contains(i.Id))
            .Where(i => i.ParentId == null)
            .Where(i => i.OwnerId != userId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<int> DeleteFilesWithPendingExpired()
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        var rowsAffected = await _context.Items
            .Where(f => f.Type == TypeOfItem.File && f.Status == UploadStatus.Pending && f.CreatedDate < oneHourAgo)
            .ExecuteDeleteAsync();
        //business logic leak but that is how it should be
        return rowsAffected;
    }

    public async Task<List<Item>> SearchItemsAsync(string searchQuery, Guid userId, CancellationToken cancellationToken = default)
    {
        var results = await _context.Items.Where(p => EF.Functions.TrigramsSimilarity(p.Name, searchQuery) > 0.3 && p.OwnerId == userId)
            .OrderByDescending(p => EF.Functions.TrigramsSimilarity(p.Name, searchQuery))
            .Take(50) //than we sort in item finder by mime type
            .ToListAsync(cancellationToken: cancellationToken);
        return results;
    }
    
}