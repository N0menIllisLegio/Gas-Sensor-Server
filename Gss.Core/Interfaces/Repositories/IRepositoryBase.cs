using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Core.Interfaces.Repositories
{
  public interface IRepositoryBase<TEntity>
    where TEntity : class
  {
    Task<TEntity> GetFirstWhereAsync(Expression<Func<TEntity, bool>> match,
     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

    Task<List<TEntity>> GetAllByWhereAsync(Expression<Func<TEntity, bool>> match, bool disableTracking = false);

    Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
      bool disableTracking = true);

    Task<PagedResultDto<TEntity>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
      Expression<Func<TEntity, object>> searchedPropertiesSelector,
      Expression<Func<TEntity, bool>> additionalFilterCriteria = null,
      bool disableTracking = true);

    Task<TEntity> FindAsync(Guid id);
    TEntity Update(TEntity entity);
    TEntity Add(TEntity entity);
    TEntity Remove(TEntity entity);
    Task<TEntity> ReloadAsync(TEntity entityToReload);
  }
}
