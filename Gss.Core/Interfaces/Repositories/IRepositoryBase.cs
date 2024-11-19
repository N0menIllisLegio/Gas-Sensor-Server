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
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

  Task<TEntity?> FindAsync(Guid id);
  void Update(TEntity entity);
  Guid Add(TEntity entity);
  Task<TEntity> ReloadAsync(TEntity entityToReload);
  Task<int> RemoveAsync(Guid entityId);
}