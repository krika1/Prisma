namespace Prisma.Core
{
    public class DatabaseSet<TEntity>
    {
        private DatabaseContext _databaseContext;

        public DatabaseSet(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool IsTracking { get; set; } = true;
        public NavigationProperty? Navigation { get; set; }

        public EntityEntry<TEntity> Add<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var entityEntry = new EntityEntry<TEntity>(entity, EntityState.Added);

            _databaseContext.ChangeTracker.Add(entityEntry);

            return entityEntry;
        }

        public EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var entry = _databaseContext.ChangeTracker.Entries.Where(x => x.GetEntity() as TEntity == entity).FirstOrDefault();

            if (entry is not null) return (EntityEntry<TEntity>)entry;

            var entityEntry = new EntityEntry<TEntity>(entity, EntityState.Unchanged);

            _databaseContext.ChangeTracker.Add(entityEntry);

            return entityEntry;
        }


        public EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var entry = _databaseContext.ChangeTracker.Entries.Where(x => x.GetEntity() as TEntity == entity).FirstOrDefault();

            if (entry is null) return null!;

            entry.State = EntityState.Deleted;

            return (EntityEntry<TEntity>)entry;
        }

        public EntityEntry<TEntity> Update<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var entry = _databaseContext.ChangeTracker.Entries.Where(x => x.GetEntity() as TEntity == entity).FirstOrDefault();

            if (entry is null) return null!;

            entry.State = EntityState.Modified;

            return (EntityEntry<TEntity>)entry;
        }

        internal async Task<object> ExecuteQueryAsync<TEntity>(QueryType queryType, NavigationProperty navigation = null)
            where TEntity : class, new()
        {
            var sql = string.Format(DatabaseTranslator.Translate(queryType), _databaseContext.GetSetPropertyName(typeof(TEntity)));

            var result = await _databaseContext.Database.ExecuteSqlAsync<TEntity>(sql);

            if (IsTracking)
            {
                foreach (var entity in result)
                {
                    var entityEntry = new EntityEntry<TEntity>(entity, EntityState.Unchanged);

                    _databaseContext.ChangeTracker.Add(entityEntry);
                }
            }

            return result;
        }

        internal async Task<object> ExecuteQueryAsync<TEntity>(QueryType queryType, string whereClause, NavigationProperty navigation = null)
            where TEntity : class, new()
        {
            var sql = string.Format(DatabaseTranslator.Translate(queryType), _databaseContext.GetSetPropertyName(typeof(TEntity)), whereClause);

            var result = await _databaseContext.Database.ExecuteSingleSqlAsync<TEntity>(sql);

            if (navigation is not null)
            {
                var navigationType = result.GetType().GetProperty(navigation.NavigationProprtyName!)!.PropertyType;

                var navigationSql = string.Format(DatabaseTranslator.Translate(queryType), _databaseContext.GetSetPropertyName(navigationType), ExpressionExtractor.SubstituteWhereForFK(whereClause, navigation.ForeignKeyReference!.ToString()!, navigation.ForeignKey!.ToString()!));

                var navigationEntity = await _databaseContext.Database.ExecuteSingleSqlAsync(navigationType, navigationSql);

                var navigationPropertyInfo = result.GetType().GetProperty(navigation.NavigationProprtyName!);

                if (navigationPropertyInfo != null)
                {
                    navigationPropertyInfo.SetValue(result, navigationEntity);
                }
            }

            if (IsTracking)
            {
                var entityEntry = new EntityEntry<TEntity>(result, EntityState.Unchanged);

                _databaseContext.ChangeTracker.Add(entityEntry);
            }

            return result;
        }
    }
}
