using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
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

  public async Task<PagedResultDto<TEntity>> GetPagedResultAsync(
    PagedInfoDto pagedInfoDto,
    Expression<Func<TEntity, bool>>? search = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    CancellationToken cancellationToken = default)
  {
    var query = search == null
      ? DbSet
      : DbSet.Where(search);

    query = query.AsNoTracking();

    query = pagedInfoDto.SortOptions is null || pagedInfoDto.SortOptions.Count == 0
      ? query.OrderBy(entity => entity.Id)
      : query.OrderBy(pagedInfoDto.SortOptions);

    if (include is not null)
    {
      query = include(query);
    }

    var totalItemsCount = await query.CountAsync(cancellationToken: cancellationToken);

    var items = await query
      .Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize)
      .Take(pagedInfoDto.PageSize)
      .ToListAsync(cancellationToken: cancellationToken);

    return new PagedResultDto<TEntity>
    {
      Items = items,
      TotalItemsCount = totalItemsCount,
      PagedInfo = pagedInfoDto
    };
  }

  public virtual async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await DbSet.FindAsync(id, cancellationToken);
  }

  public virtual void Update(TEntity entity)
  {
    DbSet.Update(entity);
  }

  public virtual Guid Add(TEntity entity)
  {
    if (entity.Id == Guid.Empty)
    {
      entity.Id = Guid.NewGuid();
    }

    return DbSet.Add(entity).Entity.Id;
  }

  public virtual async Task<int> RemoveAsync(Guid entityId, CancellationToken cancellationToken = default)
  {
    return await DbSet.Where(x => x.Id == entityId).ExecuteDeleteAsync(cancellationToken: cancellationToken);
  }

  public async Task<TEntity> ReloadAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await _context.Entry(entity).ReloadAsync(cancellationToken);
    return entity;
  }
}