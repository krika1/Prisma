using Prisma.Core.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace Prisma.Core.Extensions
{
    public static class DatabaseQueriesExtensions
    {
        public static async Task<List<TEntity>> ToListAsync<TEntity>(this DatabaseSet<TEntity> databaseSet)
            where TEntity : class, new()
        {
            return (List<TEntity>)await databaseSet.ExecuteQueryAsync<TEntity>(QueryType.SelectAll);
        }

        public static async Task<TEntity>? FirstOrDefaultAsync<TEntity>(this DatabaseSet<TEntity> databaseSet, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, new()
        {
            var tranlatedPredicate = ExpressionExtractor.ConvertExpressionToSql(predicate);

            return (TEntity)await databaseSet.ExecuteQueryAsync<TEntity>(QueryType.SelectOne, tranlatedPredicate, databaseSet.Navigation!);
        }

        public static DatabaseSet<TEntity>? AsNoTracking<TEntity>(this DatabaseSet<TEntity> databaseSet)
           where TEntity : class, new()
        {
            databaseSet.IsTracking = false;

            return databaseSet;
        }

        public static DatabaseSet<TEntity>? Include<TEntity>(this DatabaseSet<TEntity> databaseSet, string navigationProperty)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            var navigationPropInfo = entityType.GetProperty(navigationProperty);

            var relatedEntityType = navigationPropInfo!.PropertyType;

            var foreignKeyProperty = relatedEntityType
          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .FirstOrDefault(p =>
          {
              var foreignKeyAttr = p.GetCustomAttribute<ForeignKeyAttribute>();
              return foreignKeyAttr != null &&
                     foreignKeyAttr.RelatedEntityType == entityType;
          });

            var foreignKeyAttribute = foreignKeyProperty!.GetCustomAttribute<ForeignKeyAttribute>();

            databaseSet.Navigation = new(foreignKeyProperty!.Name, foreignKeyAttribute!.RelatedKey, navigationProperty);

            return databaseSet;
        }
    }
}
