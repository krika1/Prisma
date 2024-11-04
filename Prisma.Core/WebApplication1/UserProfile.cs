using Prisma.Core.Attributes;

namespace WebApplication1
{
    public class UserProfile
    {
        public int ProfileId { get; set; }

        [ForeignKeyAttribute(typeof(User), "Id")]
        public int UserId { get; set; }
    }
}
