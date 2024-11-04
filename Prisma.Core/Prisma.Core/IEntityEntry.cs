namespace Prisma.Core
{
    public interface IEntityEntry
    {
        public EntityState State { get; set; }
        object GetEntity();
    }
}
