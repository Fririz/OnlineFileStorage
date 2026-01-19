using FileApiService.Application.Contracts;
using FileApiService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileApiService.Infrastructure.Repository;

public class CachedItemRepository : IItemRepository
{
    private readonly IItemRepository _innerRepo;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedItemRepository> _logger;

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ContractResolver = new PrivateSetterContractResolver()
    };

    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public CachedItemRepository(
        IItemRepository itemRepository, 
        IDistributedCache cache,
        ILogger<CachedItemRepository> logger)
    {
        _innerRepo = itemRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string key = $"item:{id}";

        var cached = await _cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache hit for item {Id}", id);
            return JsonConvert.DeserializeObject<Item>(cached, _jsonSettings);
        }

        _logger.LogInformation("Item {Id} missing from cache, loading from DB", id);
        var item = await _innerRepo.GetByIdAsync(id, cancellationToken);
        
        if (item != null)
        {
            _logger.LogDebug("Setting cache for item {Id}", id);
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(item, _jsonSettings), _cacheOptions, cancellationToken);
        }

        return item;
    }

    public async Task<IEnumerable<Item?>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _innerRepo.GetByIdsAsync(ids, cancellationToken);
    }

    public async Task<IEnumerable<Item?>> GetAllChildrenAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        string key = $"item:{itemId}:children";

        var cached = await _cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache hit for children of {ItemId}", itemId);
            return JsonConvert.DeserializeObject<IEnumerable<Item>>(cached, _jsonSettings)!;
        }

        _logger.LogInformation("Children cache miss for {ItemId}, querying DB", itemId);
        var items = await _innerRepo.GetAllChildrenAsync(itemId, cancellationToken);
        
        var itemsList = items.ToList();
        
        if (itemsList.Any())
        {
            _logger.LogDebug("Caching {Count} children for parent {ItemId}", itemsList.Count, itemId);
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(itemsList, _jsonSettings), _cacheOptions, cancellationToken);
        }

        return itemsList;
    }

    public async Task<Item?> GetParent(Guid itemId, CancellationToken cancellationToken = default)
    {
        string key = $"item:{itemId}:parent";
        
        var cached = await _cache.GetStringAsync(key, token: cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Parent cache hit for {ItemId}", itemId);
            return JsonConvert.DeserializeObject<Item>(cached, _jsonSettings);
        }

        _logger.LogInformation("Parent cache miss for {ItemId}, fetching from DB", itemId);
        var parent = await _innerRepo.GetParent(itemId, cancellationToken);
        
        if (parent != null)
        {
             await _cache.SetStringAsync(key, JsonConvert.SerializeObject(parent, _jsonSettings), _cacheOptions, token: cancellationToken);
        }

        return parent;
    }

    public async Task<IEnumerable<Item?>> GetRootItems(Guid userId, CancellationToken cancellationToken = default)
    {
        string key = $"user:{userId}:root_items";
        
        var cached = await _cache.GetStringAsync(key, token: cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
             _logger.LogInformation("Root items cache hit for user {UserId}", userId);
             return JsonConvert.DeserializeObject<IEnumerable<Item>>(cached, _jsonSettings)!;
        }

        _logger.LogInformation("Root items cache miss for user {UserId}, loading from DB", userId);
        var items = await _innerRepo.GetRootItems(userId, cancellationToken);
        
        var itemsList = items.ToList();
        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(itemsList, _jsonSettings), _cacheOptions, token: cancellationToken);
        
        return itemsList;
    }

    public async Task<IEnumerable<Item?>> GetSharedRootItems(Guid userId, CancellationToken cancellationToken = default)
    {
        string key = $"user:{userId}:shared_items";
        
        var cached = await _cache.GetStringAsync(key, token: cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
             _logger.LogInformation("Shared items cache hit for user {UserId}", userId);
             return JsonConvert.DeserializeObject<IEnumerable<Item>>(cached, _jsonSettings)!;
        }

        _logger.LogInformation("Shared items cache miss for user {UserId}, loading from DB", userId);
        var items = await _innerRepo.GetSharedRootItems(userId, cancellationToken);
        
        var itemsList = items.ToList();
        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(itemsList, _jsonSettings), _cacheOptions, token: cancellationToken);

        return itemsList;
    }


    public async Task<Item> AddAsync(Item entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding item {Name} to DB", entity.Name);
        var addedItem = await _innerRepo.AddAsync(entity, cancellationToken);
        
        await InvalidateListCacheAsync(entity, cancellationToken);
        return addedItem;
    }

    public async Task UpdateAsync(Item entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating item {Id}", entity.Id);
        await _innerRepo.UpdateAsync(entity, cancellationToken);
        
        await InvalidateEntityCacheAsync(entity, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<Item> entities, CancellationToken cancellationToken = default)
    {
        var itemsList = entities.ToList();
        _logger.LogInformation("Bulk updating {Count} items", itemsList.Count);
        
        await _innerRepo.UpdateRangeAsync(itemsList, cancellationToken);

        foreach (var item in itemsList)
        {
            await InvalidateEntityCacheAsync(item, cancellationToken);
        }
    }

    public async Task DeleteAsync(Item entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting item {Id}", entity.Id);
        await _innerRepo.DeleteAsync(entity, cancellationToken);
        
        await InvalidateEntityCacheAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(IEnumerable<Item> entities, CancellationToken cancellationToken = default)
    {
        var itemsList = entities.ToList();
        _logger.LogInformation("Bulk deleting {Count} items", itemsList.Count);

        await _innerRepo.DeleteAsync(itemsList, cancellationToken);

        foreach (var item in itemsList)
        {
            await InvalidateEntityCacheAsync(item, cancellationToken);
        }
    }

    public Task<int> DeleteFilesWithPendingExpired()
    {
        return _innerRepo.DeleteFilesWithPendingExpired();
    }

    private async Task InvalidateEntityCacheAsync(Item entity, CancellationToken ct)
    {
        _logger.LogInformation("Invalidating cache for item {Id}", entity.Id);
        
        await _cache.RemoveAsync($"item:{entity.Id}", ct);
        await _cache.RemoveAsync($"item:{entity.Id}:parent", ct);
        
        await InvalidateListCacheAsync(entity, ct);
    }

    private async Task InvalidateListCacheAsync(Item entity, CancellationToken ct)
    {
        if (entity.ParentId == null)
        {
            var key = $"user:{entity.OwnerId}:root_items";
            _logger.LogDebug("Evicting root cache key {Key}", key);
            await _cache.RemoveAsync(key, ct);
        }
        else
        {
            var key = $"item:{entity.ParentId}:children";
            _logger.LogDebug("Evicting children cache key {Key}", key);
            await _cache.RemoveAsync(key, ct);
        }
    }

    public class PrivateSetterContractResolver : Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
    {
        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as System.Reflection.PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }
            return prop;
        }
    }
}