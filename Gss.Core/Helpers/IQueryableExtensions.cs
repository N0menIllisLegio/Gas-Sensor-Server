using System;
using System.Linq;
using System.Linq.Expressions;
using Gss.Core.Enums;

namespace Gss.Core.Helpers
{
  public static class IQueryableExtensions
  {
    public static IQueryable<T> GetPage<T>(this IQueryable<T> collection,
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<T, object>> sorter = null,
      Expression<Func<T, bool>> filter = null)
    {
      var query = collection.Where(filter ?? ((_) => true));

      if (sortOrder == SortOrder.Ascendind && sorter is not null)
      {
        query = query.OrderBy(sorter);
      }
      else if (sortOrder == SortOrder.Descending && sorter is not null)
      {
        query = query.OrderByDescending(sorter);
      }

      return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
  }
}
