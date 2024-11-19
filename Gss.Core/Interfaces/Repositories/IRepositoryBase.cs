using System.Linq.Expressions;
using Gss.Core.DTOs;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Core.Interfaces.Repositories;

public interface IRepositoryBase<TEntity>
  where TEntity : class, IEntity
{
  Task<PagedResultDto<TEntity>> GetPagedResultAsync(
    PagedInfoDto pagedInfoDto,
    Expression<Func<TEntity, bool>>? search = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    CancellationToken cancellationToken = default);

  Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default);
  void Update(TEntity entity);
  Guid Add(TEntity entity);
  Task<TEntity> ReloadAsync(TEntity entityToReload, CancellationToken cancellationToken = default);
  Task<int> RemoveAsync(Guid entityId, CancellationToken cancellationToken = default);
}