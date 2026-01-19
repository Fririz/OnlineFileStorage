using System.Data.Common;
using FileApiService.Domain.Common;
using FileApiService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FileApiService.Application.Contracts;

namespace FileApiService.Infrastructure.Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
{
    protected readonly Context _context;
    
    public RepositoryBase(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IEnumerable<T?>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
    }
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().RemoveRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }
    /*public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity  =await _context.Set<T>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }
    public async Task DeleteRangeByIdsAsync(IEnumerable<Guid> idsToDelete, CancellationToken cancellationToken = default)
    {
        var entities  = await _context.Set<T>()
            .IgnoreQueryFilters()
            .Where(x => idsToDelete.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (entities.Any())
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }*/
}