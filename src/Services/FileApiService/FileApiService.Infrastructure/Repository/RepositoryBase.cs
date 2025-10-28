using System.Data.Common;
using FileApiService.Domain.Common;
using FileApiService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FileApiService.Application.Contracts;

namespace FileApiService.Infrastructure.Repository;

public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
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

    public async Task<IReadOnlyList<T?>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync(cancellationToken);
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
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}