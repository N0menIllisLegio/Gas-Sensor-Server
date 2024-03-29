﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Infrastructure.Repositories
{
  public abstract class RepositoryBase<TEntity>: IRepositoryBase<TEntity>
    where TEntity : class, IEntity
  {
    public RepositoryBase(AppDbContext context)
    {
      Context = context;
      DbSet = context.Set<TEntity>();
    }

    protected AppDbContext Context { get; }

    protected DbSet<TEntity> DbSet { get; }

    public async Task<List<TEntity>> GetAllByWhereAsync(Expression<Func<TEntity, bool>> match,
      bool disableTracking = false)
    {
      return disableTracking
        ? await DbSet.AsNoTracking().Where(match).ToListAsync()
        : await DbSet.Where(match).ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>,
      IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
    {
      IQueryable<TEntity> query = DbSet;

      if (disableTracking)
      {
        query = query.AsNoTracking();
      }

      if (include is not null)
      {
        query = include(query);
      }

      return await query.ToListAsync();
    }

    public async Task<TEntity> GetFirstWhereAsync(Expression<Func<TEntity, bool>> match,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
    {
      return include is not null
        ? await include(DbSet).FirstOrDefaultAsync(match)
        : await DbSet.FirstOrDefaultAsync(match);
    }

    public async Task<PagedResultDto<TEntity>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
      Expression<Func<TEntity, object>> searchedPropertiesSelector,
      Expression<Func<TEntity, bool>> additionalFilterCriteria = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
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

    public virtual async Task<TEntity> FindAsync(Guid id)
    {
      return await DbSet.FirstOrDefaultAsync(entity => entity.Id == id);
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

    public async Task<TEntity> ReloadAsync(TEntity entity)
    {
      await Context.Entry(entity).ReloadAsync();
      return entity;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> match)
    {
      return await DbSet.CountAsync(match);
    }
  }
}
