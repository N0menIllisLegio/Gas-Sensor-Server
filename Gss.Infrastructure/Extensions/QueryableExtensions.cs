using System.Linq.Expressions;
using System.Reflection;
using Gss.Core.DTOs;
using Gss.Core.Utils;

namespace Gss.Infrastructure.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<TEntity> OrderBy<TEntity>(
        this IQueryable<TEntity> entities, List<SortOption>? inputtedSortOptions)
    {
        if (inputtedSortOptions is null || inputtedSortOptions.Count == 0)
            return entities;

        var existingAllowedPropertiesNames = typeof(TEntity)
            .GetProperties()
            .Where(x => x.GetCustomAttribute(typeof(AllowOrderingAttribute)) is not null)
            .Select(x => x.Name)
            .ToList();

        var allowedSortOptions = inputtedSortOptions
            .Where(x => existingAllowedPropertiesNames.Contains(x.PropertyName))
            .ToList();

        IOrderedQueryable<TEntity>? result = null;

        foreach (var sortOption in allowedSortOptions)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var orderExpression = Expression.Lambda<Func<TEntity, object>>(
                Expression.Convert(Expression.Property(parameter, sortOption.PropertyName), typeof(object)), parameter);

            if (sortOption.IsAscending)
                result = result == null
                    ? entities.OrderBy(orderExpression)
                    : result.ThenBy(orderExpression);
            else
                result = result == null
                    ? entities.OrderByDescending(orderExpression)
                    : result.ThenByDescending(orderExpression);
        }

        return result ?? entities;
    }
}