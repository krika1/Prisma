namespace Prisma.Core
{
    public class ChangeTracker
    {
        private readonly List<IEntityEntry> _entries;

        public ChangeTracker()
        {
            _entries = [];
        }

        public IReadOnlyCollection<IEntityEntry> Entries { get { return _entries.AsReadOnly(); } }

        public void Add(IEntityEntry entry)
        {
            _entries.Add(entry);
        }

        public void Clean()
        {
            _entries.Clear();
        }
    }
}
