using FileApiService.Domain.Common;
namespace FileApiService.Application.Contracts;


public interface IRepositoryBase<T> where T : EntityBase
{
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<T?>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<T> AddAsync(T entity,CancellationToken cancellationToken = default);
    public Task UpdateAsync(T entity,CancellationToken cancellationToken = default);
    public Task DeleteAsync(T entity,CancellationToken cancellationToken = default);
    public Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}