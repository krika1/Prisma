namespace Prisma.Core
{
    public class EntityEntry<TEntity> : IEntityEntry
        where TEntity : class, new()
    {
        public TEntity Entry { get; set; }
        public EntityState State { get; set; }

        public EntityEntry(TEntity entry, EntityState state)
        {
            Entry = entry;
            State = state;
        }

        public object GetEntity()
        {
            return Entry;
        }
    }
}
