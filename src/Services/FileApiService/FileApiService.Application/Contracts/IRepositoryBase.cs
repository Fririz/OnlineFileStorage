using FileApiService.Domain.Common;
namespace FileApiService.Application.Contracts;


public interface IRepositoryBase<T> where T : EntityBase
{
    
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<T> AddAsync(T entity,CancellationToken cancellationToken = default);
    public Task UpdateAsync(T entity,CancellationToken cancellationToken = default);
    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    public Task<IEnumerable<T?>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    public Task DeleteAsync(T entity,CancellationToken cancellationToken = default);
    public Task<T?> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<IEnumerable<T?>>  DeleteRangeByIdsAsync(IEnumerable<Guid> idsToDelete, CancellationToken cancellationToken = default);
    
    public Task DeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
}