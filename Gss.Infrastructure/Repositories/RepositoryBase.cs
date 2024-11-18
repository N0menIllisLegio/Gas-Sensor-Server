using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Models;
using Gss.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity>: IRepositoryBase<TEntity>
  where TEntity : class, IEntity
{
  private readonly AppDbContext _context;

  protected RepositoryBase(AppDbContext context)
  {
    _context = context;

    DbSet = context.Set<TEntity>();
  }

  protected DbSet<TEntity> DbSet { get; }

  public async Task<PagedResultDto<TEntity>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
    Expression<Func<TEntity, object>> searchedPropertiesSelector,
    Expression<Func<TEntity, bool>>? additionalFilterCriteria = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    bool disableTracking = true)
  {
    var query = DbSet.SearchBy(pagedInfoDto.SearchString, searchedPropertiesSelector, pagedInfoDto.Filters, additionalFilterCriteria);

    if (disableTracking)
    {
      query = query.AsNoTracking();
    }

    query = pagedInfoDto.SortOptions is null
      ? query.OrderBy(entity => entity.Id)
      : query.OrderBy(pagedInfoDto.SortOptions);

    if (include is not null)
    {
      query = include(query);
    }

    var pagedQuery = query.Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize).Take(pagedInfoDto.PageSize);

    int totalItemsCount = await query.CountAsync();
    var items = await pagedQuery.ToListAsync();

    return new PagedResultDto<TEntity>
    {
      Items = items,
      TotalItemsCount = totalItemsCount,
      PagedInfo = pagedInfoDto
    };
  }

  public async Task<PagedResultDto<TEntity>> GetPagedResultAsync(
    PagedInfoDto pagedInfoDto,
    Expression<Func<TEntity, bool>>? search = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
  {
    var query = search == null
      ? DbSet
      : DbSet.Where(search);

    query = query.AsNoTracking();

    query = pagedInfoDto.SortOptions is null || pagedInfoDto.SortOptions.Count == 0
      ? query.OrderBy(entity => entity.Id)
      : query.OrderByV2(pagedInfoDto.SortOptions);

    if (include is not null)
    {
      query = include(query);
    }

    var totalItemsCount = await query.CountAsync();

    var items = await query
      .Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize)
      .Take(pagedInfoDto.PageSize)
      .ToListAsync();

    return new PagedResultDto<TEntity>
    {
      Items = items,
      TotalItemsCount = totalItemsCount,
      PagedInfo = pagedInfoDto
    };
  }

  public virtual async Task<TEntity?> FindAsync(Guid id)
  {
    return await DbSet.FindAsync(id);
  }

  public virtual TEntity Update(TEntity entity)
  {
    return DbSet.Update(entity).Entity;
  }

  public virtual TEntity Add(TEntity entity)
  {
    if (entity.Id == Guid.Empty)
    {
      entity.Id = Guid.NewGuid();
    }

    return DbSet.Add(entity).Entity;
  }

  public virtual TEntity Remove(TEntity entity)
  {
    return DbSet.Remove(entity).Entity;
  }

  public virtual async Task<int> RemoveAsync(Guid entityId)
  {
    return await DbSet.Where(x => x.Id == entityId).ExecuteDeleteAsync();
  }

  public async Task<TEntity> ReloadAsync(TEntity entity)
  {
    await _context.Entry(entity).ReloadAsync();
    return entity;
  }
}