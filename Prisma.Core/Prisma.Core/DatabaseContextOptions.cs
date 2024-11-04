namespace Prisma.Core
{
    public class DatabaseContextOptions
    {
        internal string ConnectionString { get; set; } = null!;
        public void UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }

    public sealed class DatabaseContextOptions<TContext> : DatabaseContextOptions
    {
    }
}
