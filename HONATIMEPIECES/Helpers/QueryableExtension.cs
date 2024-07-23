using System.Linq.Expressions;

namespace HONATIMEPIECES.Helpers
{
    public static class QueryableExtension
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByProperty, bool desc)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(orderByProperty);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property {orderByProperty} not found on type {entityType.Name}");
            }

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.Property(arg, propertyInfo);
            var selector = Expression.Lambda(property, arg);

            var methodName = desc ? "OrderByDescending" : "OrderBy";
            var resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { entityType, propertyInfo.PropertyType },
                                            query.Expression, Expression.Quote(selector));

            return query.Provider.CreateQuery<T>(resultExp);
        }
    }
}
