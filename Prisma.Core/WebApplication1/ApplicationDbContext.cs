using Prisma.Core;

namespace WebApplication1
{
    public class ApplicationDbContext : DatabaseContext
    {
        public ApplicationDbContext(DatabaseContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DatabaseSet<User>? Users { get; set; }
        public DatabaseSet<UserProfile>? UserProfile { get; set; }
    }
}
