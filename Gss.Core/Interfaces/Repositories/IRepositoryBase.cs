using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Core.Interfaces.Repositories;

public interface IRepositoryBase<TEntity>
  where TEntity : class, IEntity
{
  Task<PagedResultDto<TEntity>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
    Expression<Func<TEntity, object>> searchedPropertiesSelector,
    Expression<Func<TEntity, bool>>? additionalFilterCriteria = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    bool disableTracking = true);

  Task<PagedResultDto<TEntity>> GetPagedResultAsync(
    int pageNumber, int pageSize,
    Expression<Func<TEntity, bool>>? search = null,
    List<SortOption>? order = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    bool disableTracking = true);

  Task<TEntity?> FindAsync(Guid id);
  TEntity Update(TEntity entity);
  TEntity Add(TEntity entity);
  TEntity Remove(TEntity entity);
  Task<TEntity> ReloadAsync(TEntity entityToReload);
  Task<int> RemoveAsync(Guid entityId);
}