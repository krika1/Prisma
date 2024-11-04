namespace Prisma.Core
{
    public class DatabaseContext
    {
        private ICollection<(Type setType, string name)> _sets = [];
        private DatabaseContextOptions _options;
        private DatabaseFacade _database;
        private ChangeTracker _changeTracker;

        public DatabaseContext(DatabaseContextOptions options)
        {
            _options = options;
            _database = new DatabaseFacade(_options.ConnectionString);
            _changeTracker = new ChangeTracker();

            DatabaseSetsInitlizer.Initlize(this, _sets);
        }

        public DatabaseFacade Database { get { return _database; } }
        public ChangeTracker ChangeTracker { get { return _changeTracker; } }

        public async Task SaveChangesAsync()
        {
            var unTrackedEntries = _changeTracker.Entries;

            foreach (var unTrackedEntry in unTrackedEntries)
            {
                if (unTrackedEntry.State == EntityState.Added)
                {
                    var sql = string.Format(
                        DatabaseTranslator.Translate(QueryType.Insert),
                        GetSetPropertyName(unTrackedEntry.GetEntity().GetType()),
                        unTrackedEntry.GetEntity().GetColumns(),
                        unTrackedEntry.GetEntity().GetValues()
                        );

                    await Database.ExecuteNonQueryAsync(sql, unTrackedEntry.GetEntity());
                }
                else if (unTrackedEntry.State == EntityState.Deleted)
                {
                    var sql = string.Format(
                        DatabaseTranslator.Translate(QueryType.Delete),
                        GetSetPropertyName(unTrackedEntry.GetEntity().GetType()),
                        unTrackedEntry.GetEntity().ConstructWhereClause()
                        );

                    await Database.ExecuteNonQueryAsync(sql, unTrackedEntry.GetEntity());
                }
                else if (unTrackedEntry.State == EntityState.Modified)
                {
                    var sql = string.Format(
                        DatabaseTranslator.Translate(QueryType.Update),
                        GetSetPropertyName(unTrackedEntry.GetEntity().GetType()),
                        unTrackedEntry.GetEntity().ConstructSetClause(),
                        unTrackedEntry.GetEntity().ConstructWhereClause()
                        );

                    await Database.ExecuteNonQueryAsync(sql, unTrackedEntry.GetEntity());
                }
            }

            _changeTracker.Clean();
        }


        internal string GetSetPropertyName(Type setType)
        {
            var tuple = _sets.Where(s => s.setType == setType).FirstOrDefault();

            return tuple!.name;
        }
    }
}
