namespace Gss.Core.Interfaces.Repositories;

public interface IRepositoryBase<TEntity>
  where TEntity : class, IEntity
{
  Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default);
  Guid Add(TEntity entity);
  Task<int> RemoveAsync(Guid entityId, CancellationToken cancellationToken = default);
}