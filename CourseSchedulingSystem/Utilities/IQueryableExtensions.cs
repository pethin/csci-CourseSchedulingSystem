using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Utilities
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ConditionalWhere<T>(
            this IQueryable<T> source,
            Func<bool> condition,
            Expression<Func<T, bool>> predicate)
        {
            if (condition())
            {
                return source.Where(predicate);
            }

            return source;
        }

        public static IQueryable<TEntity> ConditionalWhere<TEntity, TProperty>(
            this IIncludableQueryable<TEntity, TProperty> source,
            Func<bool> condition,
            Expression<Func<TEntity, bool>> predicate)
        {
            if (condition())
            {
                return source.Where(predicate);
            }

            return source;
        }
    }
}
